// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, 
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            // todo: merge the two options
            builder.Services.AddDbContext<ConfigurationDbContext>(dbContextOptionsAction);
            builder.Services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var options = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(options);
            builder.Services.AddSingleton(options);

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ResourceStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            // todo: merge the two options
            builder.Services.AddDbContext<PersistedGrantDbContext>(dbContextOptionsAction);
            builder.Services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var storeOptions = new OperationalStoreOptions();
            storeOptionsAction?.Invoke(storeOptions);
            builder.Services.AddSingleton(storeOptions);

            builder.Services.AddSingleton<TokenCleanup>();
            
            return builder;
        }

        class TokenCleanupConfig : IConfigureOptions<OperationalStoreOptions>
        {
            private readonly IApplicationLifetime _applicationLifetime;
            private readonly TokenCleanup _tokenCleanup;

            public TokenCleanupConfig(IApplicationLifetime applicationLifetime, TokenCleanup tokenCleanup)
            {
                _applicationLifetime = applicationLifetime;
                _tokenCleanup = tokenCleanup;
            }

            public TokenCleanup TokenCleanup { get; }

            public void Configure(OperationalStoreOptions options)
            {
                if (options.EnableTokenCleanup)
                {
                    _applicationLifetime.ApplicationStarted.Register(_tokenCleanup.Start);
                    _applicationLifetime.ApplicationStopping.Register(_tokenCleanup.Stop);
                }
            }
        }
    }
}