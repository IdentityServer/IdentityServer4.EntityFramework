// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework {
    internal class TokenCleanup
    {
        // This logger cross multi-thread, any UI about logger not work correctly. Why EFCore logger correctly?
        private readonly ILogger<TokenCleanup> _logger;
        // The lifetime as long as this singleton, what's scoped lifetime?
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly TimeSpan _interval;
        private CancellationTokenSource _source;

        public TokenCleanup(PersistedGrantDbContext persistedGrantDbContext, ILogger<TokenCleanup> logger, TokenCleanupOptions options)
        {
            if (persistedGrantDbContext == null) throw new ArgumentNullException(nameof(persistedGrantDbContext));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Interval < 1) throw new ArgumentException("interval must be more than 1 second");
            
            _logger = logger;
            _persistedGrantDbContext = persistedGrantDbContext;
            _interval = TimeSpan.FromSeconds(options.Interval);
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

            // Need to dispose here?
            _persistedGrantDbContext.Dispose();
        }

        private async Task Start(CancellationToken cancellationToken)
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

        private void ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                var expired = _persistedGrantDbContext.PersistedGrants.Where(x => x.Expiration < DateTimeOffset.UtcNow).ToArray();

                _logger.LogDebug("Clearing {tokenCount} tokens", expired.Length);

                if (expired.Length > 0)
                {
                    _persistedGrantDbContext.PersistedGrants.RemoveRange(expired);
                    _persistedGrantDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception cleaning tokens {exception}", ex.Message);
            }
        }
    }
}