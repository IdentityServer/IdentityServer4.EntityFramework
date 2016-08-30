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
    public class PersistedGrantStoreTests
    {
        private readonly DbContextOptions<PersistedGrantDbContext> options;

        public PersistedGrantStoreTests()
        {
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            builder.UseInMemoryDatabase();
            options = builder.Options;
        }

        private static PersistedGrant CreateTestObject()
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = Constants.PersistedGrantTypes.AuthorizationCode,
                ClientId = Guid.NewGuid().ToString(),
                SubjectId = Guid.NewGuid().ToString(),
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public void StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                store.StoreAsync(persistedGrant).Wait();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var foundGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == persistedGrant.Key);
                Assert.NotNull(foundGrant);
            }
        }

        [Fact]
        public void GetAsync_WithKeyAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant.ToEntity());
                context.SaveChanges();
            }

            PersistedGrant foundPersistedGrant;
            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                foundPersistedGrant = store.GetAsync(persistedGrant.Key).Result;
            }

            Assert.NotNull(foundPersistedGrant);
        }

        [Fact]
        public void GetAsync_WithSubAndTypeAndPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant.ToEntity());
                context.SaveChanges();
            }

            IList<PersistedGrant> foundPersistedGrants;
            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                foundPersistedGrants = store.GetAllAsync(persistedGrant.SubjectId).Result.ToList();
            }

            Assert.NotNull(foundPersistedGrants);
            Assert.NotEmpty(foundPersistedGrants);
        }

        [Fact]
        public void RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant.ToEntity());
                context.SaveChanges();
            }
            
            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                store.RemoveAsync(persistedGrant.Key).Wait();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var foundGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Fact]
        public void RemoveAsync_WhenSubIdAndClientIdOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant.ToEntity());
                context.SaveChanges();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId).Wait();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var foundGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }

        [Fact]
        public void RemoveAsync_WhenSubIdClientIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = CreateTestObject();

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant.ToEntity());
                context.SaveChanges();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                store.RemoveAllAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).Wait();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var foundGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }
    }
}