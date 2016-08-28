using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
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

        [Fact]
        public void StoreAsync_WhenPersistedGrantStored_ExpectSuccess()
        {
            var persistedGrant = new PersistedGrant
            {
                Key = "F97937D6-C934-467F-AAA8-B50D257B4E65",
                Type = Constants.PersistedGrantTypes.AuthorizationCode,
                ClientId = "test_client",
                SubjectId = "C6600243-6079-46FF-B18D-C39420C7A8C7",
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = "4B16D2BD-5B68-4B68-BB6D-EFF8449BAC21"
            };

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
        public void GetAsync_WhenPersistedGrantExists_ExpectPersistedGrantReturned()
        {
            var persistedGrant = new Entities.PersistedGrant
            {
                Key = "406DC4B7-7EA0-4BF7-B00F-CC886A4B41DF",
                Type = Constants.PersistedGrantTypes.AuthorizationCode,
                ClientId = "test_client",
                SubjectId = "C6600243-6079-46FF-B18D-C39420C7A8C7",
                CreationTime = new DateTime(2016, 08, 01),
                Expiration = new DateTime(2016, 08, 31),
                Data = "4B16D2BD-5B68-4B68-BB6D-EFF8449BAC21"
            };

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant);
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
        public void RemoveAsync_WhenKeyOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = new Entities.PersistedGrant
            {
                Key = "7D02FDDB-93A0-45C4-8DC4-A4957E9ED33D"
            };

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant);
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
        public void RemoveAsync_WhenIdAndTypeOfExistingReceived_ExpectGrantDeleted()
        {
            var persistedGrant = new Entities.PersistedGrant
            {
                Key = "44B02875-151D-4A44-BE31-32613C822B93",
                ClientId = "E41BD89C-2883-43E3-8A58-5F876D26DB33",
                SubjectId = "310C082D-8B30-4851-97FE-DAA45842BB93",
                Type = Constants.PersistedGrantTypes.AuthorizationCode
            };

            using (var context = new PersistedGrantDbContext(options))
            {
                context.PersistedGrants.Add(persistedGrant);
                context.SaveChanges();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var store = new PersistedGrantStore(context);
                store.RemoveAsync(persistedGrant.SubjectId, persistedGrant.ClientId, persistedGrant.Type).Wait();
            }

            using (var context = new PersistedGrantDbContext(options))
            {
                var foundGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == persistedGrant.Key);
                Assert.Null(foundGrant);
            }
        }
    }
}