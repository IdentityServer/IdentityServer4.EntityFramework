using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddEntityFrameworkStores(this IIdentityServerBuilder builder)
        {
            builder.AddEntityFrameworkClientStore();
            builder.AddEntityFrameworkScopeStore();

            return builder;
        }

        public static IIdentityServerBuilder AddEntityFrameworkGrantStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IPersistedGrantDbContext, PersistedGrantDbContext>();
            builder.Services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            
            return builder;
        }

        public static IIdentityServerBuilder AddEntityFrameworkClientStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientDbContext, ClientDbContext>();
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IScopeStore, ScopeStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddEntityFrameworkScopeStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IScopeDbContext, ScopeDbContext>();
            builder.Services.AddTransient<IScopeStore, ScopeStore>();

            return builder;
        }
    }
}