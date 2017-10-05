// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Interfaces
{
    /// <summary>
    /// Abstraction for the configuration context.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IConfigurationDbContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        DbSet<Client> Clients { get; set; }
        
        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        DbSet<IdentityResource> IdentityResources { get; set; }
        
        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        DbSet<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}