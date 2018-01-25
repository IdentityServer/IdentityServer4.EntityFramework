// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    public class ScopeStoreTests : IClassFixture<DatabaseProviderFixture<ConfigurationDbContext>>
    {
        private static readonly ConfigurationStoreOptions StoreOptions = new ConfigurationStoreOptions();
        public static readonly TheoryData<DbContextOptions<ConfigurationDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ConfigurationDbContext>>
        {
            DatabaseProviderBuilder.BuildInMemory<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions),
            DatabaseProviderBuilder.BuildSqlite<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions),
            DatabaseProviderBuilder.BuildLocalDb<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions)
        };

        public ScopeStoreTests(DatabaseProviderFixture<ConfigurationDbContext> fixture)
        {
            fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ConfigurationDbContext>)y)).ToList();
            fixture.StoreOptions = StoreOptions;
        }

        private static IdentityResource CreateIdentityTestResource()
        {
            return new IdentityResource()
            {
                Name = Guid.NewGuid().ToString(),
                DisplayName = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowInDiscoveryDocument = true,
                UserClaims = 
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateApiTestResource()
        {
            return new ApiResource()
            {
                Name = Guid.NewGuid().ToString(),
                ApiSecrets = new List<Secret> {new Secret("secret".Sha256())},
                Scopes =
                    new List<Scope>
                    {
                        new Scope
                        {
                            Name = Guid.NewGuid().ToString(),
                            UserClaims = {Guid.NewGuid().ToString()}
                        }
                    },
                UserClaims = 
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                }
            };
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindResourcesAsync_WhenResourcesExist_ExpectResourcesReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.IdentityResources.Add(testIdentityResource.ToEntity());
                context.ApiResources.Add(testApiResource.ToEntity());
                context.SaveChanges();
            }

            Resources resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name,
                    testApiResource.Scopes.First().Name
                }).Result;
            }

            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.NotNull(resources.IdentityResources.FirstOrDefault(x => x.Name == testIdentityResource.Name));
            Assert.NotNull(resources.ApiResources.FirstOrDefault(x => x.Name == testApiResource.Name));
        }
        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindResourcesAsync_WhenResourcesExist_ExpectOnlyResourcesRequestedReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var testIdentityResource = CreateIdentityTestResource();
            var testApiResource = CreateApiTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.IdentityResources.Add(testIdentityResource.ToEntity());
                context.ApiResources.Add(testApiResource.ToEntity());
                context.IdentityResources.Add(CreateIdentityTestResource().ToEntity());
                context.ApiResources.Add(CreateApiTestResource().ToEntity());
                context.SaveChanges();
            }

            Resources resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindResourcesByScopeAsync(new List<string>
                {
                    testIdentityResource.Name,
                    testApiResource.Scopes.First().Name
                }).Result;
            }

            Assert.NotNull(resources);
            Assert.NotNull(resources.IdentityResources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotNull(resources.ApiResources);
            Assert.NotEmpty(resources.ApiResources);
            Assert.Equal(1, resources.IdentityResources.Count);
            Assert.Equal(1, resources.ApiResources.Count);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void GetAllResources_WhenAllResourcesRequested_ExpectAllResourcesIncludingHidden(DbContextOptions<ConfigurationDbContext> options)
        {
            var visibleIdentityResource = CreateIdentityTestResource();
            var visibleApiResource = CreateApiTestResource();
            var hiddenIdentityResource = new IdentityResource{Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false};
            var hiddenApiResource = new ApiResource
            {
                Name = Guid.NewGuid().ToString(),
                Scopes = new List<Scope> {new Scope {Name = Guid.NewGuid().ToString(), ShowInDiscoveryDocument = false}}
            };

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.IdentityResources.Add(visibleIdentityResource.ToEntity());
                context.ApiResources.Add(visibleApiResource.ToEntity());
                context.IdentityResources.Add(hiddenIdentityResource.ToEntity());
                context.ApiResources.Add(hiddenApiResource.ToEntity());
                context.SaveChanges();
            }

            Resources resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.GetAllResourcesAsync().Result;
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources.IdentityResources);
            Assert.NotEmpty(resources.ApiResources);

            Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
            Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindIdentityResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var resource = CreateIdentityTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.IdentityResources.Add(resource.ToEntity());
                context.SaveChanges();
            }

            IList<IdentityResource> resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindIdentityResourcesByScopeAsync(new List<string>
                {
                    resource.Name
                }).Result.ToList();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            var foundScope = resources.Single();

            Assert.Equal(resource.Name, foundScope.Name);
            Assert.NotNull(foundScope.UserClaims);
            Assert.NotEmpty(foundScope.UserClaims);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindIdentityResourcesByScopeAsync_WhenResourcesExist_ExpectOnlyRequestedReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var resource = CreateIdentityTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.IdentityResources.Add(resource.ToEntity());
                context.IdentityResources.Add(CreateIdentityTestResource().ToEntity());
                context.SaveChanges();
            }

            IList<IdentityResource> resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindIdentityResourcesByScopeAsync(new List<string>
                {
                    resource.Name
                }).Result.ToList();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Equal(1, resources.Count);
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindApiResourceAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var resource = CreateApiTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.ApiResources.Add(resource.ToEntity());
                context.SaveChanges();
            }

            ApiResource foundResource;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                foundResource = store.FindApiResourceAsync(resource.Name).Result;
            }

            Assert.NotNull(foundResource);

            Assert.NotNull(foundResource.UserClaims);
            Assert.NotEmpty(foundResource.UserClaims);
            Assert.NotNull(foundResource.ApiSecrets);
            Assert.NotEmpty(foundResource.ApiSecrets);
            Assert.NotNull(foundResource.Scopes);
            Assert.NotEmpty(foundResource.Scopes);
            Assert.Contains(foundResource.Scopes, x => x.UserClaims.Any());
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindApiResourcesByScopeAsync_WhenResourceExists_ExpectResourceAndCollectionsReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var resource = CreateApiTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.ApiResources.Add(resource.ToEntity());
                context.SaveChanges();
            }

            IList<ApiResource> resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindApiResourcesByScopeAsync(new List<string> {resource.Scopes.First().Name}).Result.ToList();
            }

            Assert.NotEmpty(resources);
            Assert.NotNull(resources);

            Assert.NotNull(resources.First().UserClaims);
            Assert.NotEmpty(resources.First().UserClaims);
            Assert.NotNull(resources.First().ApiSecrets);
            Assert.NotEmpty(resources.First().ApiSecrets);
            Assert.NotNull(resources.First().Scopes);
            Assert.NotEmpty(resources.First().Scopes);
            Assert.Contains(resources.First().Scopes, x => x.UserClaims.Any());
        }

        [Theory, MemberData(nameof(TestDatabaseProviders))]
        public void FindApiResourcesByScopeAsync_WhenMultipleResourcesExist_ExpectOnlyRequestedResourcesReturned(DbContextOptions<ConfigurationDbContext> options)
        {
            var resource = CreateApiTestResource();

            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                context.ApiResources.Add(resource.ToEntity());
                context.ApiResources.Add(CreateApiTestResource().ToEntity());
                context.ApiResources.Add(CreateApiTestResource().ToEntity());
                context.SaveChanges();
            }

            IList<ApiResource> resources;
            using (var context = new ConfigurationDbContext(options, StoreOptions))
            {
                var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
                resources = store.FindApiResourcesByScopeAsync(new List<string> {resource.Scopes.First().Name}).Result.ToList();
            }

            Assert.NotNull(resources);
            Assert.NotEmpty(resources);
            Assert.Equal(1, resources.Count);
        }
    }
}