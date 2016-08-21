using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.DbContexts
{
    public class ScopeDbContextTests
    {
        [Fact]
        public void WhenScopeAddedToContext_ExpectSuccess()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:"};
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            var optionsBuilder = new DbContextOptionsBuilder<ScopeDbContext>();
            optionsBuilder.UseSqlite(connection);
            var builtOptions = optionsBuilder.Options;

            var testScope = new Scope { Name = "testScope" };

            using (var context = new ScopeDbContext(builtOptions))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
                //context.Database.Migrate();
            }

            using (var context = new ScopeDbContext(builtOptions))
            {
                context.Scopes.Add(testScope);
                context.SaveChanges();
            }

            using (var context = new ScopeDbContext(builtOptions))
            {
                var foundScope = context.Scopes.FirstOrDefault(x => x.Name == testScope.Name);
                Assert.Equal(testScope.Name, foundScope.Name);
            }
        }
    }
}