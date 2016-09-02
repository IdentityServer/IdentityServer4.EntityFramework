using System.Linq;
using Host.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Stores;
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
            const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Test.IdentityServer4.EntityFramework;Integrated Security=true";

            services.AddMvc();

            services.AddIdentityServer()
                .SetTemporarySigningCredential()
                .AddInMemoryUsers(Users.Get());

            services.AddDbContext<ClientDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Host")));
            services.AddTransient<IClientStore, ClientStore>();

            services.AddDbContext<ScopeDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Host")));
            services.AddTransient<IScopeStore, ScopeStore>();

            services.AddDbContext<PersistedGrantDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Host")));
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
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