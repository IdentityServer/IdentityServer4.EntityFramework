using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.DbContexts
{
    public class ClientDbContextTests : IClassFixture<DatabaseProviderFixture<ClientDbContext>>
    {
        public static readonly TheoryData<DbContextOptions<ClientDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ClientDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<ClientDbContext>(nameof(ClientDbContextTests)),
            DatabaseProviderBuilder.BuildSqlite<ClientDbContext>(nameof(ClientDbContextTests)),
            DatabaseProviderBuilder.BuildSqlServer<ClientDbContext>(nameof(ClientDbContextTests))
        };

        public ClientDbContextTests(DatabaseProviderFixture<ClientDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ClientDbContext>)y)).ToList();
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void CanAddAndDeleteClientScopes(DbContextOptions<ClientDbContext> options)
        {
            using (var db = new ClientDbContext(options))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client-scopes",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                // explicit include due to lack of EF Core lazy loading
                var client = db.Clients.Include(x => x.AllowedScopes).First();

                client.AllowedScopes.Add(new ClientScope
                {
                    Scope = "test"
                });

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                var client = db.Clients.Include(x => x.AllowedScopes).First();
                var scope = client.AllowedScopes.First();

                client.AllowedScopes.Remove(scope);

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                var client = db.Clients.Include(x => x.AllowedScopes).First();

                Assert.Empty(client.AllowedScopes);
            }
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void CanAddAndDeleteClientRedirectUri(DbContextOptions<ClientDbContext> options)
        {
            using (var db = new ClientDbContext(options))
            {
                db.Clients.Add(new Client
                {
                    ClientId = "test-client",
                    ClientName = "Test Client"
                });

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();

                client.RedirectUris.Add(new ClientRedirectUri
                {
                    RedirectUri = "https://redirect-uri-1"
                });

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();
                var redirectUri = client.RedirectUris.First();

                client.RedirectUris.Remove(redirectUri);

                db.SaveChanges();
            }

            using (var db = new ClientDbContext(options))
            {
                var client = db.Clients.Include(x => x.RedirectUris).First();

                Assert.Equal(0, client.RedirectUris.Count());
            }
        }
    }
}