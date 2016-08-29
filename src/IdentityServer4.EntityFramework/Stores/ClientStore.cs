using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IClientDbContext context;

        public ClientStore(IClientDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            this.context = context;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await context.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .FirstOrDefaultAsync(x => x.ClientId == clientId);

            return client.ToModel();
        }
    }
}