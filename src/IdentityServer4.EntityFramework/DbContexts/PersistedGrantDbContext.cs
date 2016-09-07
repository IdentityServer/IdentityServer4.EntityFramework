using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.DbContexts
{
    public class PersistedGrantDbContext : DbContext, IPersistedGrantDbContext
    {
        public PersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options) : base(options) { }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigurePersistedGrantContext();

            base.OnModelCreating(modelBuilder);
        }
    }
}