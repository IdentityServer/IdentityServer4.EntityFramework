// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using IdentityServer4.Quickstart.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Host
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IHostingEnvironment HostingEnvironment;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            Configuration = config;
            HostingEnvironment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("db");

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddTestUsers(TestUsers.Users)
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    //options.ResolveDbContextOptions = (provider, builder) =>
                    //{
                    //    builder.UseSqlServer(cn);
                    //};
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds, short for testing
                })
                //.AddOperationalStoreNotification<TestOperationalStoreNotification>()
                .AddConfigurationStoreCache();

            services.AddMvc();

            return services.BuildServiceProvider(validateScopes: true);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
