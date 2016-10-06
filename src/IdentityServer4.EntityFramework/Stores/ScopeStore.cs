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
    public class ScopeStore : IScopeStore
    {
        private readonly IConfigurationDbContext _context;
        private readonly ILogger<ScopeStore> _logger;

        public ScopeStore(IConfigurationDbContext context, ILogger<ScopeStore> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _logger = logger;
        }

        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            IQueryable<Entities.Scope> scopes = _context.Scopes
                .Include(x => x.Claims)
                .Include(x => x.ScopeSecrets);

            if (scopeNames != null && scopeNames.Any())
            {
                scopes = scopes.Where(x => scopeNames.Contains(x.Name));
            }

            var foundScopes = scopes.ToList();

            _logger.LogDebug("Found {scopes} scopes in database", foundScopes.Select(x => x.Name));

            var model = foundScopes.Select(x => x.ToModel());

            return Task.FromResult(model);
        }

        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            IQueryable<Entities.Scope> scopes = _context.Scopes
                .Include(x => x.Claims)
                .Include(x => x.ScopeSecrets);

            if (publicOnly)
            {
                scopes = scopes.Where(x => x.ShowInDiscoveryDocument);
            }

            var foundScopes = scopes.ToList();

            _logger.LogDebug("Found {scopes} scopes in database", foundScopes.Select(x => x.Name));

            var model = foundScopes.Select(x => x.ToModel());

            return Task.FromResult(model);
        }
    }
}