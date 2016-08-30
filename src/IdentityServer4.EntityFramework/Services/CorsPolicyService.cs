using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IClientDbContext context;

        public CorsPolicyService(IClientDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            this.context = context;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var origins = await context.Clients.SelectMany(x => x.AllowedCorsOrigins.Select(y => y.Origin)).ToListAsync();

            var distinctOrigins = origins.Where(x => x != null).Distinct();
            
            return distinctOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        }
    }
}