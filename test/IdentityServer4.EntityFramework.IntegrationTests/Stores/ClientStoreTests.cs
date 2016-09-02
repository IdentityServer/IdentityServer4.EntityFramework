using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ClientStoreTests : IClassFixture<DatabaseProviderFixture<ClientDbContext>>
    {
        public static readonly TheoryData<DbContextOptions<ClientDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ClientDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<ClientDbContext>(nameof(ClientStoreTests)),
            DatabaseProviderBuilder.BuildSqlite<ClientDbContext>(nameof(ClientStoreTests)),
            DatabaseProviderBuilder.BuildSqlServer<ClientDbContext>(nameof(ClientStoreTests))
        };

        public ClientStoreTests(DatabaseProviderFixture<ClientDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ClientDbContext>)y)).ToList();
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured(DbContextOptions<ClientDbContext> options)
        {
            var testClient = new Client
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            using (var context = new ClientDbContext(options))
            {
                context.Clients.Add(testClient.ToEntity());
                context.SaveChanges();
            }

            Client client;
            using (var context = new ClientDbContext(options))
            {
                var store = new ClientStore(context);
                client = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            Assert.NotNull(client);
        }
    }
}