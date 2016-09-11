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
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly ILogger _logger;

        public PersistedGrantStore(IPersistedGrantDbContext context, ILogger<PersistedGrantStore> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task StoreAsync(PersistedGrant token)
        {
            var existing = _context.PersistedGrants.SingleOrDefault(x => x.Key == token.Key);
            if (existing == null)
            {
                var persistedGrant = token.ToEntity();
                _context.PersistedGrants.Add(persistedGrant);
            }
            else
            {
                token.UpdateEntity(existing);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "StoreAsync");
            }

            return Task.FromResult(0);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = _context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            var model = persistedGrant.ToModel();

            return Task.FromResult(model);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrants = _context.PersistedGrants.Where(x => x.SubjectId == subjectId).ToList();
            var model = persistedGrants.Select(x => x.ToModel());

            return Task.FromResult(model);
        }

        public Task RemoveAsync(string key)
        {
            var persistedGrant = _context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            if (persistedGrant!= null)
            {
                _context.PersistedGrants.Remove(persistedGrant);
                _context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = _context.PersistedGrants.Where(x => x.SubjectId == subjectId && x.ClientId == clientId).ToList();

            _context.PersistedGrants.RemoveRange(persistedGrants);
            _context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = _context.PersistedGrants.Where(x =>
                x.SubjectId == subjectId &&
                x.ClientId == clientId &&
                x.Type == type).ToList();

            _context.PersistedGrants.RemoveRange(persistedGrants);
            _context.SaveChanges();

            return Task.FromResult(0);
        }
    }
}