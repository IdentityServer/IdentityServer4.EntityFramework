using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Interfaces
{
    public interface IPersistedGrantDbContext
    {
        DbSet<PersistedGrant> PersistedGrants { get; set; }
        Task<int> SaveChangesAsync();
    }
}