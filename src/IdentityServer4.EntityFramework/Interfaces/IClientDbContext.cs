using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Interfaces
{
    public interface IClientDbContext
    {
        DbSet<Client> Clients { get; set; }
    }
}