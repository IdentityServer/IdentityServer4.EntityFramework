using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ClientStoreTests
    {
        private readonly DbContextOptions<ClientDbContext> options;
        private readonly Client testClient = new Client {ClientId = "test_client"};

        public ClientStoreTests()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            var builder = new DbContextOptionsBuilder<ClientDbContext>();
            builder.UseSqlite(connection);
            options = builder.Options;

            using (var context = new ClientDbContext(options))
            {
                // Create tables
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add test data
                context.Clients.Add(testClient.ToEntity());
                context.SaveChanges();
            }
        }

        [Fact]
        public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured()
        {
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