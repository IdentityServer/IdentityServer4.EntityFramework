using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;

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

        public async Task StoreAsync(PersistedGrant token)
        {
            var persistedGrant = token.ToEntity();
            context.PersistedGrants.Add(persistedGrant);
            await context.SaveChangesAsync();
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await context.PersistedGrants.FirstOrDefaultAsync(x => x.Key == key);
            return persistedGrant.ToModel();
        }

        public async Task RemoveAsync(string key)
        {
            var persistedGrant = await context.PersistedGrants.FirstOrDefaultAsync(x => x.Key == key);
            if (persistedGrant!= null)
            {
                context.PersistedGrants.Remove(persistedGrant);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(string subjectId, string clientId, string type)
        {
            var persistedGrants = await context.PersistedGrants.Where(x =>
                x.SubjectId == subjectId &&
                x.ClientId == clientId &&
                x.Type == type).ToListAsync();

            context.PersistedGrants.RemoveRange(persistedGrants);
            await context.SaveChangesAsync();
        }
    }
}