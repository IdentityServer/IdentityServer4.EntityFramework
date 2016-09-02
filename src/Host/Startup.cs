using System;
using System.Linq;
using Host.Configuration;
using Host.Extensions;
using Host.UI.Login;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Quickstart;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Test.IdentityServer4.EntityFramework;Integrated Security=true";

            var builder = services.AddIdentityServer(options =>
                {
                    options.UserInteractionOptions.LoginUrl = "/ui/login";
                    options.UserInteractionOptions.LogoutUrl = "/ui/logout";
                    options.UserInteractionOptions.ConsentUrl = "/ui/consent";
                    options.UserInteractionOptions.ErrorUrl = "/ui/error";
                })
                .SetTemporarySigningCredential()
                .AddInMemoryStores()
                .AddInMemoryScopes(Scopes.Get());

            // UI service for in-memory users
            services.AddSingleton(new InMemoryUserLoginService(Users.Get()));
            builder.AddResourceOwnerValidator<InMemoryUserResourceOwnerPasswordValidator>();

            services.AddDbContext<ClientDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Host")));
            services.AddTransient<IClientStore, ClientStore>();

            services.AddDbContext<ScopeDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Host")));


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

            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ClientDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<ScopeDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ClientDbContext>());
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ScopeDbContext>());
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public static void EnsureSeedData(ClientDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.Get().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
        }

        public static void EnsureSeedData(ScopeDbContext context)
        {
            if (!context.Scopes.Any())
            {
                foreach (var client in Scopes.Get().ToList())
                {
                    context.Scopes.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}