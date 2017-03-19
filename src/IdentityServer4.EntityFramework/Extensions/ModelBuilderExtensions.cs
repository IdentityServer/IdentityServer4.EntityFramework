// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer4.EntityFramework.Extensions
{

    public static class ModelBuilderExtensions
    {
        private static EntityTypeBuilder<TEntity> ToTable<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, TableConfiguration configuration)
            where TEntity : class
        {
            return string.IsNullOrWhiteSpace(configuration.Schema) ? entityTypeBuilder.ToTable(configuration.Name) : entityTypeBuilder.ToTable(configuration.Name, configuration.Schema);
        }

        public static void ConfigureClientContext(this ModelBuilder modelBuilder, ConfigurationStoreOptions storeOptions)
        {
            if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<Client>(client =>
            {
                client.ToTable(storeOptions.Client);
                client.HasKey(x => x.Id);

                client.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                client.Property(x => x.ProtocolType).HasMaxLength(200).IsRequired();
                client.Property(x => x.ClientName).HasMaxLength(200);
                client.Property(x => x.ClientUri).HasMaxLength(2000);

                client.HasIndex(x => x.ClientId).IsUnique();

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
                grantType.ToTable(storeOptions.ClientGrantType);
                grantType.Property(x => x.GrantType).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientRedirectUri>(redirectUri =>
            {
                redirectUri.ToTable(storeOptions.ClientRedirectUri);
                redirectUri.Property(x => x.RedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUri>(postLogoutRedirectUri =>
            {
                postLogoutRedirectUri.ToTable(storeOptions.ClientPostLogoutRedirectUri);
                postLogoutRedirectUri.Property(x => x.PostLogoutRedirectUri).HasMaxLength(2000).IsRequired();
            });

            modelBuilder.Entity<ClientScope>(scope =>
            {
                scope.ToTable(storeOptions.ClientScopes);
                scope.Property(x => x.Scope).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientSecret>(secret =>
            {
                secret.ToTable(storeOptions.ClientSecret);
                secret.Property(x => x.Value).HasMaxLength(2000).IsRequired();
                secret.Property(x => x.Type).HasMaxLength(250);
                secret.Property(x => x.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<ClientClaim>(claim =>
            {
                claim.ToTable(storeOptions.ClientClaim);
                claim.Property(x => x.Type).HasMaxLength(250).IsRequired();
                claim.Property(x => x.Value).HasMaxLength(250).IsRequired();
            });

            modelBuilder.Entity<ClientIdPRestriction>(idPRestriction =>
            {
                idPRestriction.ToTable(storeOptions.ClientIdPRestriction);
                idPRestriction.Property(x => x.Provider).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ClientCorsOrigin>(corsOrigin =>
            {
                corsOrigin.ToTable(storeOptions.ClientCorsOrigin);
                corsOrigin.Property(x => x.Origin).HasMaxLength(150).IsRequired();
            });
        }

        public static void ConfigurePersistedGrantContext(this ModelBuilder modelBuilder, OperationalStoreOptions storeOptions)
        {
            if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<PersistedGrant>(grant =>
            {
                grant.ToTable(storeOptions.PersistedGrants);

                grant.Property(x => x.Key).HasMaxLength(200).ValueGeneratedNever();
                grant.Property(x => x.Type).HasMaxLength(50).IsRequired();
                grant.Property(x => x.SubjectId).HasMaxLength(200);
                grant.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
                grant.Property(x => x.CreationTime).IsRequired();
                grant.Property(x => x.Data).IsRequired();

                grant.HasKey(x => x.Key);

                grant.HasIndex(x => new { x.SubjectId, x.ClientId, x.Type });
            });
        }

        public static void ConfigureResourcesContext(this ModelBuilder modelBuilder, ConfigurationStoreOptions storeOptions)
        {
            if (!string.IsNullOrWhiteSpace(storeOptions.DefaultSchema)) modelBuilder.HasDefaultSchema(storeOptions.DefaultSchema);

            modelBuilder.Entity<IdentityResource>(identityResource =>
            {
                identityResource.ToTable(storeOptions.IdentityResource).HasKey(x => x.Id);

                identityResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                identityResource.Property(x => x.DisplayName).HasMaxLength(200);
                identityResource.Property(x => x.Description).HasMaxLength(1000);

                identityResource.HasIndex(x => x.Name).IsUnique();

                identityResource.HasMany(x => x.UserClaims).WithOne(x => x.IdentityResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<IdentityClaim>(claim =>
            {
                claim.ToTable(storeOptions.IdentityClaim).HasKey(x => x.Id);

                claim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });


            modelBuilder.Entity<ApiResource>(apiResource =>
            {
                apiResource.ToTable(storeOptions.ApiResource).HasKey(x => x.Id);

                apiResource.Property(x => x.Name).HasMaxLength(200).IsRequired();
                apiResource.Property(x => x.DisplayName).HasMaxLength(200);
                apiResource.Property(x => x.Description).HasMaxLength(1000);

                apiResource.HasIndex(x => x.Name).IsUnique();

                apiResource.HasMany(x => x.Secrets).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.Scopes).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
                apiResource.HasMany(x => x.UserClaims).WithOne(x => x.ApiResource).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiSecret>(apiSecret =>
            {
                apiSecret.ToTable(storeOptions.ApiSecret).HasKey(x => x.Id);

                apiSecret.Property(x => x.Description).HasMaxLength(1000);
                apiSecret.Property(x => x.Value).HasMaxLength(2000);
                apiSecret.Property(x => x.Type).HasMaxLength(250);
            });

            modelBuilder.Entity<ApiResourceClaim>(apiClaim =>
            {
                apiClaim.ToTable(storeOptions.ApiClaim).HasKey(x => x.Id);

                apiClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });

            modelBuilder.Entity<ApiScope>(apiScope =>
            {
                apiScope.ToTable(storeOptions.ApiScope).HasKey(x => x.Id);

                apiScope.Property(x => x.Name).HasMaxLength(200).IsRequired();
                apiScope.Property(x => x.DisplayName).HasMaxLength(200);
                apiScope.Property(x => x.Description).HasMaxLength(1000);

                apiScope.HasIndex(x => x.Name).IsUnique();

                apiScope.HasMany(x => x.UserClaims).WithOne(x => x.ApiScope).IsRequired().OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ApiScopeClaim>(apiScopeClaim =>
            {
                apiScopeClaim.ToTable(storeOptions.ApiScopeClaim).HasKey(x => x.Id);

                apiScopeClaim.Property(x => x.Type).HasMaxLength(200).IsRequired();
            });
        }
    }
}
