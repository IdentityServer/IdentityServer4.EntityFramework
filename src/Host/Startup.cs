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

namespace Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework;trusted_connection=yes;";
            
            services.AddMvc();

            services.AddIdentityServer()
                .SetTemporarySigningCredential()
                .AddInMemoryUsers(Users.Get())
                .AddEntityFrameworkStores()
                .AddEntityFrameworkGrantStore();

            services.AddDbContext<ClientDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly(Assembly.GetEntryAssembly().GetName().Name)));
            services.AddDbContext<ScopeDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly(Assembly.GetEntryAssembly().GetName().Name)));
            services.AddDbContext<PersistedGrantDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly(Assembly.GetEntryAssembly().GetName().Name)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            app.UseDeveloperExceptionPage();
            
            // Setup Databases
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ClientDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<ScopeDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ClientDbContext>());
                EnsureSeedData(serviceScope.ServiceProvider.GetService<ScopeDbContext>());

                //var dbContextOptions = app.ApplicationServices.GetRequiredService<DbContextOptions<PersistedGrantDbContext>>();
                var options = serviceScope.ServiceProvider.GetService<DbContextOptions<PersistedGrantDbContext>>();
                var tokenCleanup = new TokenCleanup(options);
                tokenCleanup.Start();
            }

            app.UseIdentityServer();
            
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void EnsureSeedData(ClientDbContext context)
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

        private static void EnsureSeedData(ScopeDbContext context)
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