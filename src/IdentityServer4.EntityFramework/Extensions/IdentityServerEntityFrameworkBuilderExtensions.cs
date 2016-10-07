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

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerEntityFrameworkBuilderExtensions
    {
        public static IIdentityServerBuilder AddConfigurationStore(
            this IIdentityServerBuilder builder, 
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
            Action<ConfigurationStoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddDbContext<ConfigurationDbContext>(dbContextOptionsAction);
            builder.Services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IScopeStore, ScopeStore>();
            builder.Services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            var options = new ConfigurationStoreOptions();
            storeOptionsAction?.Invoke(options);
            builder.Services.AddSingleton(options);

            return builder;
        }

        public static IIdentityServerBuilder AddConfigurationStoreCache(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddMemoryCache(); // TODO: remove once update idsvr since it does this
            builder.AddInMemoryCaching();

            // these need to be registered as concrete classes in DI for
            // the caching decorators to work
            builder.Services.AddTransient<ClientStore>();
            builder.Services.AddTransient<ScopeStore>();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddScopeStoreCache<ScopeStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddOperationalStore(
            this IIdentityServerBuilder builder,
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
            Action<OperationalStoreOptions> storeOptionsAction = null)
        {
            builder.Services.AddDbContext<PersistedGrantDbContext>(dbContextOptionsAction);
            builder.Services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();

            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            var options = new OperationalStoreOptions();
            storeOptionsAction?.Invoke(options);
            builder.Services.AddSingleton(options);

            return builder;
        }
    }
}