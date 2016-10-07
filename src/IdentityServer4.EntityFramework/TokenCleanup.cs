// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework
{
    public class TokenCleanupOptions
    {
        public DbContextOptions<PersistedGrantDbContext> DbContextOptions { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public int Interval { get; set; } = 60;
    }

    public class TokenCleanup
    {
        private readonly DbContextOptions<PersistedGrantDbContext> _options;
        private readonly TimeSpan _interval;
        private CancellationTokenSource _source;
        private readonly ILogger _logger;

        public TokenCleanup(TokenCleanupOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.DbContextOptions == null) throw new ArgumentNullException(nameof(options.DbContextOptions));
            if (options.Interval < 1) throw new ArgumentException("interval must be more than 1 second");

            _options = options.DbContextOptions;
            _interval = TimeSpan.FromSeconds(options.Interval);
            _logger = options.LoggerFactory?.CreateLogger(typeof(TokenCleanup).FullName) ?? new NopLogger();
        }

        public void Start()
        {
            if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

            _logger.LogDebug("Starting token cleanup");

            _source = new CancellationTokenSource();
            Task.Factory.StartNew(() => Start(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

            _logger.LogDebug("Stopping token cleanup");

            _source.Cancel();
            _source = null;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                try
                {
                    await Task.Delay(_interval, cancellationToken);
                }
                catch
                {
                    _logger.LogDebug("Task.Delay exception. exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested");
                    break;
                }

                ClearTokens();
            }
        }

        protected virtual IPersistedGrantDbContext CreateOperationalDbContext()
        {
            // TODO: Replace with DI.
            return new PersistedGrantDbContext(_options, new OperationalStoreOptions());
        }

        private void ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                using (var context = CreateOperationalDbContext())
                {
                    var expired = context.PersistedGrants.Where(x => x.Expiration < DateTimeOffset.UtcNow).ToArray();

                    _logger.LogDebug("Clearing {tokenCount} tokens", expired.Length);

                    context.PersistedGrants.RemoveRange(expired);

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception cleaning tokens {exception}", ex.Message);
            }
        }
    }
}