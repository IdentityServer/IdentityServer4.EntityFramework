using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Interfaces
{
    public interface IScopeDbContext
    {
        DbSet<Scope> Scopes { get; set; }
    }
}