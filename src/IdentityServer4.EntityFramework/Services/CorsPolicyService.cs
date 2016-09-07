using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly ConfigurationDbContext context;

        public CorsPolicyService(ConfigurationDbContext context)
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