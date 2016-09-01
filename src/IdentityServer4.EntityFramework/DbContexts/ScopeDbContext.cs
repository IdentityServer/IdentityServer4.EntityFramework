using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IdentityServer4.EntityFramework.DbContexts
{
    public class ScopeDbContext : DbContext, IScopeDbContext
    {
        public ScopeDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Scope> Scopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScopeClaim>(scopeClaim =>
            {
                scopeClaim.ToTable(EfConstants.TableNames.ScopeClaim).HasKey(x => x.Id);
                scopeClaim.Property(x => x.Name).HasMaxLength(200).IsRequired();
                scopeClaim.Property(x => x.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<ScopeSecret>(scopeSecret =>
            {
                scopeSecret.ToTable(EfConstants.TableNames.ScopeSecrets).HasKey(x => x.Id);
                scopeSecret.Property(x => x.Description).HasMaxLength(1000);
                scopeSecret.Property(x => x.Type).HasMaxLength(250);
                scopeSecret.Property(x => x.Value).HasMaxLength(250);
            });

            modelBuilder.Entity<Scope>(scope =>
            {
                scope.ToTable(EfConstants.TableNames.Scope).HasKey(x => x.Id);
                scope.Property(x => x.Name).HasMaxLength(200).IsRequired();
                scope.Property(x => x.DisplayName).HasMaxLength(200);
                scope.Property(x => x.Description).HasMaxLength(1000);
                scope.Property(x => x.ClaimsRule).HasMaxLength(200);
                scope.HasMany(x => x.Claims).WithOne(x => x.Scope).OnDelete(DeleteBehavior.Cascade);
                scope.HasMany(x => x.ScopeSecrets).WithOne(x => x.Scope).OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}