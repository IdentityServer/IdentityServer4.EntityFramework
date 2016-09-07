// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}