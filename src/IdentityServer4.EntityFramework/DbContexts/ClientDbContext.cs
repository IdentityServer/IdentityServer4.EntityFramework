using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            });
            

            base.OnModelCreating(modelBuilder);
        }
    }
}