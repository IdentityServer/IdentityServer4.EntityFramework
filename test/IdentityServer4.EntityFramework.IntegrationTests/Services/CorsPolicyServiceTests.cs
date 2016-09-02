using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Services;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace IdentityServer4.EntityFramework.IntegrationTests.Services
{
    public class CorsPolicyServiceTests : IClassFixture<DatabaseProviderFixture<ClientDbContext>>
    {
        public static readonly TheoryData<DbContextOptions<ClientDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ClientDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<ClientDbContext>(nameof(CorsPolicyServiceTests)),
            DatabaseProviderBuilder.BuildSqlite<ClientDbContext>(nameof(CorsPolicyServiceTests)),
            DatabaseProviderBuilder.BuildSqlServer<ClientDbContext>(nameof(CorsPolicyServiceTests))
        };

        public CorsPolicyServiceTests(DatabaseProviderFixture<ClientDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ClientDbContext>)y)).ToList();
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void IsOriginAllowedAsync_WhenOriginIsAllowed_ExpectTrue(DbContextOptions<ClientDbContext> options)
        {
            const string testCorsOrigin = "https://identityserver.io/";

            using (var context = new ClientDbContext(options))
            {
                context.Clients.Add(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }.ToEntity());
                context.Clients.Add(new Client
                {
                    ClientId = "2",
                    ClientName = "2",
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com", testCorsOrigin }
                }.ToEntity());
                context.SaveChanges();
            }

            bool result;
            using (var context = new ClientDbContext(options))
            {
                var service = new CorsPolicyService(context);
                result = service.IsOriginAllowedAsync(testCorsOrigin).Result;
            }

            Assert.True(result);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void IsOriginAllowedAsync_WhenOriginIsNotAllowed_ExpectFalse(DbContextOptions<ClientDbContext> options)
        {
            using (var context = new ClientDbContext(options))
            {
                context.Clients.Add(new Client
                {
                    ClientId = Guid.NewGuid().ToString(),
                    ClientName = Guid.NewGuid().ToString(),
                    AllowedCorsOrigins = new List<string> { "https://www.identityserver.com" }
                }.ToEntity());
                context.SaveChanges();
            }

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