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

namespace IdentityServer4.EntityFramework.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantDbContext context;

        public PersistedGrantStore(IPersistedGrantDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            this.context = context;
        }

        public Task StoreAsync(PersistedGrant token)
        {
            var persistedGrant = token.ToEntity();
            context.PersistedGrants.Add(persistedGrant);
            context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            var model = persistedGrant.ToModel();

            return Task.FromResult(model);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrants = context.PersistedGrants.Where(x => x.SubjectId == subjectId).ToList();
            var model = persistedGrants.Select(x => x.ToModel());

            return Task.FromResult(model);
        }

        public Task RemoveAsync(string key)
        {
            var persistedGrant = context.PersistedGrants.FirstOrDefault(x => x.Key == key);
            if (persistedGrant!= null)
            {
                context.PersistedGrants.Remove(persistedGrant);
                context.SaveChanges();
            }

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var persistedGrants = context.PersistedGrants.Where(x => x.SubjectId == subjectId && x.ClientId == clientId).ToList();

            context.PersistedGrants.RemoveRange(persistedGrants);
            context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = context.PersistedGrants.Where(x =>
                x.SubjectId == subjectId &&
                x.ClientId == clientId &&
                x.Type == type).ToList();

            context.PersistedGrants.RemoveRange(persistedGrants);
            context.SaveChanges();

            return Task.FromResult(0);
        }
    }
}