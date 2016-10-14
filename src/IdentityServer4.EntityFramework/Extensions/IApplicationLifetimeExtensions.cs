using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.EntityFramework.Extensions {
    public static class IApplicationLifetimeExtensions
    {
        // Maybe this have any better solution?
        public static IApplicationLifetime AttachIdentityServerTokenCleanup(this IApplicationLifetime appLifetime, IServiceProvider serviceProvider) 
        {
            var tokenCleanup = serviceProvider.GetService<TokenCleanup>();
            // If the token clean up disabled, this will be null
            if(tokenCleanup != null) {
                appLifetime.ApplicationStarted.Register(tokenCleanup.Start);
                appLifetime.ApplicationStopping.Register(tokenCleanup.Stop);
            }
            
            return appLifetime;
        }
    }
}
