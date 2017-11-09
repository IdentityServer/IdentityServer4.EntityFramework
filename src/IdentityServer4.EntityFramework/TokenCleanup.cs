// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework
{
    internal class TokenCleanup
    {
        private readonly ILogger<TokenCleanup> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly OperationalStoreOptions _options;

        private CancellationTokenSource _source;

        public TimeSpan CleanupInterval => TimeSpan.FromSeconds(_options.TokenCleanupInterval);

        public TokenCleanup(IServiceProvider serviceProvider, ILogger<TokenCleanup> logger, OperationalStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupInterval < 1) throw new ArgumentException("Token cleanup interval must be at least 1 second");
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Start()
        {
            Start(CancellationToken.None);
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

            _logger.LogDebug("Starting token cleanup");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Factory.StartNew(() => StartInternal(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

            _logger.LogDebug("Stopping token cleanup");

            _source.Cancel();
            _source = null;
        }

        private async Task StartInternal(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Task.Delay exception: {0}. Exiting.", ex.Message);
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                ClearTokens();
            }
        }

        public void ClearTokens()
        {
            try
            {
                _logger.LogTrace("Querying for tokens to clear");

                var found = Int32.MaxValue;
                
                using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<IPersistedGrantDbContext>())
                    {
                        while (found >= _options.TokenCleanupBatchSize)
                        {
                            var expired = context.PersistedGrants
                                .Where(x => x.Expiration < DateTime.UtcNow)
                                .Take(_options.TokenCleanupBatchSize)
                                .ToArray();

                            found = expired.Length;
                            _logger.LogInformation("Clearing {tokenCount} tokens", found);

                            if (found > 0)
                            {
                                context.PersistedGrants.RemoveRange(expired);
                                try
                                {
                                    context.SaveChanges();
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    // we get this if/when someone else already deleted the records
                                    // we want to essentially ignore this, and keep working
                                    _logger.LogDebug("Concurrency exception clearing tokens: {exception}", ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception clearing tokens: {exception}", ex.Message);
            }
        }
    }
}