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
            builder.UseSqlite($"Filename=./Test.IdentityServer4.EntityFramework-2.0.0.{name}.db");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();
            }

            return options;
        }

        public static DbContextOptions<T> BuildLocalDb<T>(string name, object tableOptions) where T : DbContext
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

        public static DbContextOptions<T> BuildAppVeyorSqlServer2016<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer($@"Server=(local)\SQL2016;Database=Test.IdentityServer4.EntityFramework-2.0.0.{name};User ID=sa;Password=Password12!");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }

        public static DbContextOptions<T> BuildAppVeyorMySql<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseMySql($@"server=localhost;database=Test.IdentityServer4.EntityFramework-2.0.0.{name};userid=root;pwd=Password12!;port=3306;persistsecurityinfo=True;");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }

        public static DbContextOptions<T> BuildAppVeyorPostgreSql<T>(string name, object tableOptions) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseNpgsql($@"Host=localhost;Database=Test.IdentityServer4.EntityFramework-2.0.0.{name};Username=postgres;Password=Password12!;Port=5432;");
            var options = builder.Options;

            using (var context = (T)Activator.CreateInstance(typeof(T), options, tableOptions))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }
    }
}