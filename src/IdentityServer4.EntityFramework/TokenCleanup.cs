// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework {
    internal class TokenCleanup
    {
        private readonly TimeSpan _interval;
        private CancellationTokenSource _source;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public TokenCleanup(OperationalStoreOptions.TokenCleanupOptions options, IServiceProvider serviceProvider)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Interval < 1) throw new ArgumentException("interval must be more than 1 second");

            _serviceProvider = serviceProvider;
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

        private void ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                // 1. CreateScope() each clear token cycle.
                // 2. CreateScope() in Start(), and dispose the 'serviceScope' in Stop().
                // 
                // I choose solution 1.

                // PersistedGrantDbContext is scoped lifetime.
                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>())
                    {
                        var expired = context.PersistedGrants.Where(x => x.Expiration < DateTimeOffset.UtcNow).ToArray();

                        _logger.LogDebug("Clearing {tokenCount} tokens", expired.Length);

                        if (expired.Length > 0) 
                        {
                            context.PersistedGrants.RemoveRange(expired);

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception cleaning tokens {exception}", ex.Message);
            }
        }
    }
}