using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IdentityServer4.EntityFramework.DbContexts
{
    public class ClientDbContext : DbContext, IClientDbContext
    {
        public ClientDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable(EfConstants.TableNames.Client);
                client.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.RedirectUris).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Claims).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientGrantType>(grantType =>
            {
                grantType.ToTable(EfConstants.TableNames.ClientGrantType);
            });

            modelBuilder.Entity<ClientRedirectUri>(redirectUri =>
            {
                redirectUri.ToTable(EfConstants.TableNames.ClientRedirectUri);
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(postLogoutRedirectUri =>
            {
                postLogoutRedirectUri.ToTable(EfConstants.TableNames.ClientPostLogoutRedirectUri);
            });

            modelBuilder.Entity<ClientScope>(scope =>
            {
                scope.ToTable(EfConstants.TableNames.ClientScopes);
            });

            modelBuilder.Entity<ClientSecret>(secret =>
            {
                secret.ToTable(EfConstants.TableNames.ClientSecret);
            });

            modelBuilder.Entity<ClientClaim>(claim =>
            {
                claim.ToTable(EfConstants.TableNames.ClientClaim);
            });

            modelBuilder.Entity<ClientIdPRestriction>(clientIdPRestriction =>
            {
                clientIdPRestriction.ToTable(EfConstants.TableNames.ClientIdPRestriction);
            });

            modelBuilder.Entity<ClientCorsOrigin>(corsOrigin =>
            {
                corsOrigin.ToTable(EfConstants.TableNames.ClientCorsOrigin);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}