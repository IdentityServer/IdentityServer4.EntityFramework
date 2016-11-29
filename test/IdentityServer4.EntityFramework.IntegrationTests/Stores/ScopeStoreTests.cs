// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.Stores
{
    //public class ScopeStoreTests : IClassFixture<DatabaseProviderFixture<ConfigurationDbContext>>
    //{
    //    private static readonly ConfigurationStoreOptions StoreOptions = new ConfigurationStoreOptions();
    //    public static readonly TheoryData<DbContextOptions<ConfigurationDbContext>> TestDatabaseProviders = new TheoryData<DbContextOptions<ConfigurationDbContext>>
    //    {
    //        DatabaseProviderBuilder.BuildInMemory<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions),
    //        DatabaseProviderBuilder.BuildSqlite<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions),
    //        DatabaseProviderBuilder.BuildSqlServer<ConfigurationDbContext>(nameof(ScopeStoreTests), StoreOptions)
    //    };
        
    //    public ScopeStoreTests(DatabaseProviderFixture<ConfigurationDbContext> fixture)
    //    {
    //        fixture.Options = TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<ConfigurationDbContext>)y)).ToList();
    //        fixture.StoreOptions = StoreOptions;
    //    }

    //    private static Scope CreateTestObject()
    //    {
    //        return new Scope
    //        {
    //            Name = Guid.NewGuid().ToString(),
    //            Type = ScopeType.Identity,
    //            ShowInDiscoveryDocument = true,
    //            ScopeSecrets = new List<Secret> {new Secret("secret".Sha256())},
    //            Claims = new List<ScopeClaim>
    //            {
    //                new ScopeClaim("name"),
    //                new ScopeClaim("role")
    //            }
    //        };
    //    }

    //    [Theory, MemberData(nameof(TestDatabaseProviders))]
    //    public void FindScopesAsync_WhenScopesExist_ExpectScopesReturned(DbContextOptions<ConfigurationDbContext> options)
    //    {
    //        var firstTestScope = CreateTestObject();
    //        var secondTestScope = CreateTestObject();

    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            context.Scopes.Add(firstTestScope.ToEntity());
    //            context.Scopes.Add(secondTestScope.ToEntity());
    //            context.SaveChanges();
    //        }

    //        IList<Scope> scopes;
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
    //            scopes = store.FindScopesAsync(new List<string>
    //            {
    //                firstTestScope.Name,
    //                secondTestScope.Name
    //            }).Result.ToList();
    //        }

    //        Assert.NotNull(scopes);
    //        Assert.NotEmpty(scopes);
    //        Assert.NotNull(scopes.FirstOrDefault(x => x.Name == firstTestScope.Name));
    //        Assert.NotNull(scopes.FirstOrDefault(x => x.Name == secondTestScope.Name));
    //    }

    //    [Theory, MemberData(nameof(TestDatabaseProviders))]
    //    public void GetScopesAsync_WhenAllScopesRequested_ExpectAllScopes(DbContextOptions<ConfigurationDbContext> options)
    //    {
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            context.Scopes.Add(CreateTestObject().ToEntity());
    //            context.Scopes.Add(new Entities.ApiScope { Name = "hidden_scope_return", ShowInDiscoveryDocument = false });
    //            context.SaveChanges();
    //        }

    //        IList<Scope> scopes;
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
    //            scopes = store.GetScopesAsync(false).Result.ToList();
    //        }

    //        Assert.NotNull(scopes);
    //        Assert.NotEmpty(scopes);

    //        Assert.True(scopes.Any(x => !x.ShowInDiscoveryDocument));
    //    }

    //    [Theory, MemberData(nameof(TestDatabaseProviders))]
    //    public void GetScopesAsync_WhenAllDiscoveryScopesRequested_ExpectAllDiscoveryScopes(DbContextOptions<ConfigurationDbContext> options)
    //    {
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            context.Scopes.Add(CreateTestObject().ToEntity());
    //            context.Scopes.Add(new Entities.ApiScope { Name = "hidden_scope", ShowInDiscoveryDocument = false });
    //            context.SaveChanges();
    //        }

    //        IList<Scope> scopes;
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
    //            scopes = store.GetScopesAsync().Result.ToList();
    //        }

    //        Assert.NotNull(scopes);
    //        Assert.NotEmpty(scopes);

    //        Assert.True(scopes.All(x => x.ShowInDiscoveryDocument));
    //    }

    //    [Theory, MemberData(nameof(TestDatabaseProviders))]
    //    public void FindScopesAsync_WhenScopeHasSecrets_ExpectScopeAndSecretsReturned(DbContextOptions<ConfigurationDbContext> options)
    //    {
    //        var scope = CreateTestObject();

    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            context.Scopes.Add(scope.ToEntity());
    //            context.SaveChanges();
    //        }

    //        IList<Scope> scopes;
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
    //            scopes = store.FindScopesAsync(new List<string>
    //            {
    //                scope.Name
    //            }).Result.ToList();
    //        }

    //        Assert.NotNull(scopes);
    //        var foundScope = scopes.Single();

    //        Assert.NotNull(foundScope.ScopeSecrets);
    //        Assert.NotEmpty(foundScope.ScopeSecrets);
    //    }

    //    [Theory, MemberData(nameof(TestDatabaseProviders))]
    //    public void FindScopesAsync_WhenScopeHasClaims_ExpectScopeAndClaimsReturned(DbContextOptions<ConfigurationDbContext> options)
    //    {
    //        var scope = CreateTestObject();

    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            context.Scopes.Add(scope.ToEntity());
    //            context.SaveChanges();
    //        }

    //        IList<Scope> scopes;
    //        using (var context = new ConfigurationDbContext(options, StoreOptions))
    //        {
    //            var store = new ResourceStore(context, FakeLogger<ResourceStore>.Create());
    //            scopes = store.FindScopesAsync(new List<string>
    //            {
    //                scope.Name
    //            }).Result.ToList();
    //        }

    //        Assert.NotNull(scopes);
    //        var foundScope = scopes.Single();

    //        Assert.NotNull(foundScope.Claims);
    //        Assert.NotEmpty(foundScope.Claims);
    //    }
    //}
}