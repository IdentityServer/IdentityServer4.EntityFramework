using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ScopeStoreTests
    {
        private readonly DbContextOptions<ScopeDbContext> options;

        private readonly Scope testIdentityScope = StandardScopes.OpenId;

        private readonly Scope testResourceScope = new Scope
        {
            Name = "resource_scope",
            Type = ScopeType.Resource,
            ScopeSecrets = new List<Secret> {new Secret("secret".Sha512())}
        };

        public ScopeStoreTests()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            var builder = new DbContextOptionsBuilder<ScopeDbContext>();
            builder.UseSqlite(connection);
            options = builder.Options;

            using (var context = new ScopeDbContext(options))
            {
                // Create tables
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add test data
                context.Scopes.Add(testIdentityScope.ToEntity());
                context.Scopes.Add(testResourceScope.ToEntity());
                context.Scopes.Add(new Entities.Scope {Name = "hidden_scope", ShowInDiscoveryDocument = false});
                context.SaveChanges();
            }
        }

        [Fact]
        public void FindScopesAsync_WhenScopesExist_ExpectScopesReturned()
        {
            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    testIdentityScope.Name,
                    testResourceScope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            Assert.NotEmpty(scopes);
            Assert.NotNull(scopes.FirstOrDefault(x => x.Name == testIdentityScope.Name));
            Assert.NotNull(scopes.FirstOrDefault(x => x.Name == testResourceScope.Name));
        }

        [Fact]
        public void GetScopesAsync_WhenAllScopesRequested_ExpectAllScopes()
        {
            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.GetScopesAsync(false).Result.ToList();
            }

            Assert.NotNull(scopes);
            Assert.NotEmpty(scopes);

            Assert.True(scopes.Any(x => !x.ShowInDiscoveryDocument));
        }

        [Fact]
        public void GetScopesAsync_WhenAllDiscoveryScopesRequested_ExpectAllDiscovryScopes()
        {
            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.GetScopesAsync().Result.ToList();
            }

            Assert.NotNull(scopes);
            Assert.NotEmpty(scopes);

            Assert.True(scopes.All(x => x.ShowInDiscoveryDocument));
        }

        [Fact]
        public void FindScopesAsync_WhenScopeHasSecrets_ExpectScopeAndSecretsReturned()
        {
            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    testResourceScope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            var foundScope = scopes.Single();

            Assert.NotNull(foundScope.ScopeSecrets);
            Assert.NotEmpty(foundScope.ScopeSecrets);
        }

        [Fact]
        public void FindScopesAsync_WhenScopeHasClaims_ExpectScopeAndClaimsReturned()
        {
            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    testIdentityScope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            var foundScope = scopes.Single();

            Assert.NotNull(foundScope.Claims);
            Assert.NotEmpty(foundScope.Claims);
        }
    }
}