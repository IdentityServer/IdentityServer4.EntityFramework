using System;
using Host.Configuration;
using Host.Extensions;
using Host.UI.Login;
using IdentityServer4.Quickstart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddIdentityServer(options =>
                {
                    options.UserInteractionOptions.LoginUrl = "/ui/login";
                    options.UserInteractionOptions.LogoutUrl = "/ui/logout";
                    options.UserInteractionOptions.ConsentUrl = "/ui/consent";
                    options.UserInteractionOptions.ErrorUrl = "/ui/error";
                })
                .SetTemporarySigningCredential()
                .AddInMemoryStores()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get());

            // UI service for in-memory users
            services.AddSingleton(new InMemoryUserLoginService(Users.Get()));
            builder.AddResourceOwnerValidator<InMemoryUserResourceOwnerPasswordValidator>();

            builder.AddExtensionGrantValidator<ExtensionGrantValidator>();

            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // serilog filter
            Func<LogEvent, bool> serilogFilter = e =>
            {
                var context = e.Properties["SourceContext"].ToString();

                return context.StartsWith("\"IdentityServer") ||
                       context.StartsWith("\"IdentityModel") ||
                       (e.Level == LogEventLevel.Error) ||
                       (e.Level == LogEventLevel.Fatal);
            };

            // built-in logging filter
            Func<string, LogLevel, bool> filter = (scope, level) =>
                scope.StartsWith("IdentityServer") ||
                scope.StartsWith("IdentityModel") ||
                (level == LogLevel.Error) ||
                (level == LogLevel.Critical);

            loggerFactory.AddConsole(filter);
            loggerFactory.AddDebug(filter);

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}