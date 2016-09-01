using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ScopeStoreTests : IClassFixture<DatabaseProviderFixture<ScopeDbContext>>
    {
        public static readonly TheoryData<DbContextOptions<ScopeDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ScopeDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<ScopeDbContext>(nameof(ScopeStoreTests)),
            DatabaseProviderBuilder.BuildSqlite<ScopeDbContext>(nameof(ScopeStoreTests)),
            DatabaseProviderBuilder.BuildSqlServer<ScopeDbContext>(nameof(ScopeStoreTests))
        };
        
        public ScopeStoreTests(DatabaseProviderFixture<ScopeDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ScopeDbContext>)y)).ToList();
        }

        private static Scope CreateTestObject()
        {
            return new Scope
            {
                Name = Guid.NewGuid().ToString(),
                Type = ScopeType.Identity,
                ShowInDiscoveryDocument = true,
                ScopeSecrets = new List<Secret> {new Secret("secret".Sha256())},
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim("name"),
                    new ScopeClaim("role")
                }
            };
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindScopesAsync_WhenScopesExist_ExpectScopesReturned(DbContextOptions options)
        {
            var firstTestScope = CreateTestObject();
            var secondTestScope = CreateTestObject();

            using (var context = new ScopeDbContext(options))
            {
                context.Scopes.Add(firstTestScope.ToEntity());
                context.Scopes.Add(secondTestScope.ToEntity());
                context.SaveChanges();
            }

            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    firstTestScope.Name,
                    secondTestScope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            Assert.NotEmpty(scopes);
            Assert.NotNull(scopes.FirstOrDefault(x => x.Name == firstTestScope.Name));
            Assert.NotNull(scopes.FirstOrDefault(x => x.Name == secondTestScope.Name));
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void GetScopesAsync_WhenAllScopesRequested_ExpectAllScopes(DbContextOptions options)
        {
            using (var context = new ScopeDbContext(options))
            {
                context.Scopes.Add(CreateTestObject().ToEntity());
                context.Scopes.Add(new Entities.Scope { Name = "hidden_scope_return", ShowInDiscoveryDocument = false });
                context.SaveChanges();
            }

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

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void GetScopesAsync_WhenAllDiscoveryScopesRequested_ExpectAllDiscoveryScopes(DbContextOptions options)
        {
            using (var context = new ScopeDbContext(options))
            {
                context.Scopes.Add(CreateTestObject().ToEntity());
                context.Scopes.Add(new Entities.Scope { Name = "hidden_scope", ShowInDiscoveryDocument = false });
                context.SaveChanges();
            }

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

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindScopesAsync_WhenScopeHasSecrets_ExpectScopeAndSecretsReturned(DbContextOptions options)
        {
            var scope = CreateTestObject();

            using (var context = new ScopeDbContext(options))
            {
                context.Scopes.Add(scope.ToEntity());
                context.SaveChanges();
            }

            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    scope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            var foundScope = scopes.Single();

            Assert.NotNull(foundScope.ScopeSecrets);
            Assert.NotEmpty(foundScope.ScopeSecrets);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindScopesAsync_WhenScopeHasClaims_ExpectScopeAndClaimsReturned(DbContextOptions options)
        {
            var scope = CreateTestObject();

            using (var context = new ScopeDbContext(options))
            {
                context.Scopes.Add(scope.ToEntity());
                context.SaveChanges();
            }

            IList<Scope> scopes;
            using (var context = new ScopeDbContext(options))
            {
                var store = new ScopeStore(context);
                scopes = store.FindScopesAsync(new List<string>
                {
                    scope.Name
                }).Result.ToList();
            }

            Assert.NotNull(scopes);
            var foundScope = scopes.Single();

            Assert.NotNull(foundScope.Claims);
            Assert.NotEmpty(foundScope.Claims);
        }
    }
}