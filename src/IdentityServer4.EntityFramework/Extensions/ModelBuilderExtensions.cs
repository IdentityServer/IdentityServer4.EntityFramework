// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IdentityServer4.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureClientContext(this ModelBuilder modelBuilder, ConfigurationStoreOptions storeOptions)
        {
            if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable(EfConstants.TableNames.Client).HasKey(x => x.Id);

                client.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                client.Property(x => x.ClientName).HasMaxLength(200).IsRequired();
                client.Property(x => x.ClientUri).HasMaxLength(2000);

                client.HasMany(x => x.AllowedGrantTypes).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.RedirectUris).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedScopes).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.ClientSecrets).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.Claims).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.IdentityProviderRestrictions).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
                client.HasMany(x => x.AllowedCorsOrigins).WithOne(x => x.Client).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientGrantType>(grantType =>
            {
                grantType.ToTable(EfConstants.TableNames.ClientGrantType);
                grantType.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientRedirectUri>(redirectUri =>
            {
                redirectUri.ToTable(EfConstants.TableNames.ClientRedirectUri);
                redirectUri.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(postLogoutRedirectUri =>
            {
                postLogoutRedirectUri.ToTable(EfConstants.TableNames.ClientPostLogoutRedirectUri);
                postLogoutRedirectUri.Property(x => x.PostLogoutRedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientScope>(scope =>
            {
                scope.ToTable(EfConstants.TableNames.ClientScopes);
                scope.Property(x => x.Scope).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientSecret>(secret =>
            {
                secret.ToTable(EfConstants.TableNames.ClientSecret);
                secret.Property(x => x.Value).HasMaxLength(250).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(250);
                secret.Property(x => x.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<ClientClaim>(claim =>
            {
                claim.ToTable(EfConstants.TableNames.ClientClaim);
                claim.Property(x => x.Type).HasMaxLength(250).IsRequired();
                claim.Property(x => x.Value).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientIdPRestriction>(idPRestriction =>
            {
                idPRestriction.ToTable(EfConstants.TableNames.ClientIdPRestriction);
                idPRestriction.Property(x => x.Provider).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientCorsOrigin>(corsOrigin =>
            {
                corsOrigin.ToTable(EfConstants.TableNames.ClientCorsOrigin);
                corsOrigin.Property(x => x.Origin).HasMaxLength(150).IsRequired();
            });
        }

        public static void ConfigurePersistedGrantContext(this ModelBuilder modelBuilder, OperationalStoreOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.DefaultSchema)) modelBuilder.HasDefaultSchema(options.DefaultSchema);

            modelBuilder.Entity<PersistedGrant>(grant =>
            {
                grant.ToTable(EfConstants.TableNames.PersistedGrant);
                grant.HasKey(x => new {x.Key, x.Type});
                grant.Property(x => x.SubjectId);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.CreationTime).IsRequired();
                grant.Property(x => x.Expiration).IsRequired();
                grant.Property(x => x.Data).IsRequired();
            });
        }

        public static void ConfigureScopeContext(this ModelBuilder modelBuilder, ConfigurationStoreOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.DefaultSchema)) modelBuilder.HasDefaultSchema(options.DefaultSchema);

            modelBuilder.Entity<ScopeClaim>(scopeClaim =>
            {
                scopeClaim.ToTable(EfConstants.TableNames.ScopeClaim).HasKey(x => x.Id);
                scopeClaim.Property(x => x.Name).HasMaxLength(200).IsRequired();
                scopeClaim.Property(x => x.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<ScopeSecret>(scopeSecret =>
            {
                scopeSecret.ToTable(EfConstants.TableNames.ScopeSecrets).HasKey(x => x.Id);
                scopeSecret.Property(x => x.Description).HasMaxLength(1000);
                scopeSecret.Property(x => x.Type).HasMaxLength(250);
                scopeSecret.Property(x => x.Value).HasMaxLength(250);
            });

            modelBuilder.Entity<Scope>(scope =>
            {
                scope.ToTable(EfConstants.TableNames.Scope).HasKey(x => x.Id);
                scope.Property(x => x.Name).HasMaxLength(200).IsRequired();
                scope.Property(x => x.DisplayName).HasMaxLength(200);
                scope.Property(x => x.Description).HasMaxLength(1000);
                scope.Property(x => x.ClaimsRule).HasMaxLength(200);
                scope.HasMany(x => x.Claims).WithOne(x => x.Scope).IsRequired().OnDelete(DeleteBehavior.Cascade);
                scope.HasMany(x => x.ScopeSecrets).WithOne(x => x.Scope).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
