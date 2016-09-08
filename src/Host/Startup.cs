// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Reflection;
using Host.Configuration;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework;trusted_connection=yes;";
            
            services.AddMvc();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .SetTemporarySigningCredential()
                .AddInMemoryUsers(Users.Get())

                .AddConfigurationStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))

                .AddOperationalStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(@"c:\logs\IdentityServer4.EntityFramework.Host.txt")
                .CreateLogger();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            //app.UseDeveloperExceptionPage();
            
            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ConfigurationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ConfigurationDbContext>());

                //var dbContextOptions = app.ApplicationServices.GetRequiredService<DbContextOptions<PersistedGrantDbContext>>();
                var options = serviceScope.ServiceProvider.GetService<DbContextOptions<PersistedGrantDbContext>>();
                //var tokenCleanup = new TokenCleanup(options);
                //tokenCleanup.Start();
            }

            app.UseIdentityServer();
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients.Get().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

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