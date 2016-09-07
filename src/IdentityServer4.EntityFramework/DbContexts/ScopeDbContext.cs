using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.DbContexts
{
    public class ScopeDbContext : DbContext, IScopeDbContext
    {
        public ScopeDbContext(DbContextOptions<ScopeDbContext> options) : base(options) { }

        public DbSet<Scope> Scopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureScopeContext();

            base.OnModelCreating(modelBuilder);
        }
    }
}