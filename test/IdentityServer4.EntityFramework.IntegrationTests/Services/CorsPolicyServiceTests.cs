using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Services
{
    public class CorsPolicyServiceTests
    {
        public CorsPolicyServiceTests()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = ":memory:"};
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
                context.Clients.Add(
                    new Client
                    {
                        ClientId = "1",
                        ClientName = "1",
                        AllowedCorsOrigins = new List<string> {"https://www.identityserver.com"}
                    }.ToEntity());
                context.Clients.Add(new Client
                {
                    ClientId = "2",
                    ClientName = "2",
                    AllowedCorsOrigins = new List<string> {"https://www.identityserver.com", TestCorsOrigin}
                }.ToEntity());

                context.SaveChanges();
            }
        }

        private readonly DbContextOptions<ClientDbContext> options;

        private const string TestCorsOrigin = "https://identityserver.io";

        [Fact]
        public void IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue()
        {
            bool result;
            using (var context = new ClientDbContext(options))
            {
                var service = new CorsPolicyService(context);
                result = service.IsOriginAllowedAsync(TestCorsOrigin).Result;
            }

            Assert.True(result);
        }

        [Fact]
        public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse()
        {
            bool result;
            using (var context = new ClientDbContext(options))
            {
                var service = new CorsPolicyService(context);
                result = service.IsOriginAllowedAsync("InvalidOrigin").Result;
            }

            Assert.False(result);
        }
    }
}