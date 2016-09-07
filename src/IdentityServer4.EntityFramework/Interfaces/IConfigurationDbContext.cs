using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Interfaces
{
    public interface IConfigurationDbContext : IDisposable
    {
        DbSet<Client> Clients { get; set; }
        DbSet<Scope> Scopes { get; set; }
        Task<int> SaveChangesAsync();
    }
}