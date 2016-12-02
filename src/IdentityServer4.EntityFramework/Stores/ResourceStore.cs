// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly IConfigurationDbContext _context;
        private readonly ILogger<ResourceStore> _logger;

        public ResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _logger = logger;
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            var query =
                from apiResource in _context.ApiResources
                where apiResource.Name == name
                select apiResource;

            var apis = query
                .Include(x => x.Secrets)
                .Include(x => x.Scopes)
                    .ThenInclude(s => s.UserClaims)
                .Include(x => x.UserClaims);

            var api = apis.FirstOrDefault();

            if (api != null)
            {
                _logger.LogDebug("Found {api} API resource in database", name);
            }
            else
            {
                _logger.LogDebug("Did not find {api} API resource in database", name);
            }

            return Task.FromResult(api.ToModel());
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var query =
                from api in _context.ApiResources
                let scopes = api.Scopes.Select(x => x.Name)
                where scopes.Intersect(names).Any()
                select api;

            var apis = query
                .Include(x => x.Secrets)
                .Include(x => x.Scopes)
                    .ThenInclude(s => s.UserClaims)
                .Include(x => x.UserClaims);

            var results = apis.ToArray();
            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {scopes} API scopes in database", models.SelectMany(x => x.Scopes).Select(x => x.Name));

            return Task.FromResult(models.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from identityResource in _context.IdentityResources
                where scopes.Contains(identityResource.Name)
                select identityResource;

            var resources = query
                .Include(x => x.UserClaims);

            var results = resources.ToArray();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return Task.FromResult(results.Select(x => x.ToModel()).ToArray().AsEnumerable());
        }

        public Task<Resources> GetAllResources()
        {
            var identity = _context.IdentityResources
              .Include(x => x.UserClaims);

            var apis = _context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.Scopes)
                    .ThenInclude(s => s.UserClaims)
                .Include(x => x.UserClaims);

            var result = new Resources(
                identity.ToArray().Select(x => x.ToModel()).AsEnumerable(),
                apis.ToArray().Select(x => x.ToModel()).AsEnumerable());

            _logger.LogDebug("Found {scopes} as all scopes in database", result.IdentityResources.Select(x=>x.Name).Union(result.ApiResources.SelectMany(x=>x.Scopes).Select(x=>x.Name)));

            return Task.FromResult(result);
        }
    }
}