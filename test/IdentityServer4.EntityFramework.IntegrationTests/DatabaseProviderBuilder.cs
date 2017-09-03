// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.IntegrationTests
{
    /// <summary>
    /// Helper methods to initialize DbContextOptions for the specified database provider and context.
    /// </summary>
    public class DatabaseProviderBuilder
    {
        public static DbContextOptions<T> BuildInMemory<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseInMemoryDatabase(name);
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }

        public static DbContextOptions<T> BuildSqlite<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite($"Filename=./Test.IdentityServer4.EntityFramework.{name}-2.0.0.db");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
            }

            return options;
        }

        public static DbContextOptions<T> BuildSqlServer<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(
                $@"Data Source=(LocalDb)\MSSQLLocalDB;database=Test.IdentityServer4.EntityFramework-2.0.0.{name};trusted_connection=yes;");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }
    }
}