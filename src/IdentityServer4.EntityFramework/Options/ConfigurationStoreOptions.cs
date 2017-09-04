// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Options
{
    public class ConfigurationStoreOptions
    {
        public Action<DbContextOptionsBuilder> ConfigureDbContext { get; set; }

        public string DefaultSchema { get; set; } = null;

        public TableConfiguration IdentityResource { get; set; } = new TableConfiguration("IdentityResources");
        public TableConfiguration IdentityClaim { get; set; } = new TableConfiguration("IdentityClaims");

        public TableConfiguration ApiResource { get; set; } = new TableConfiguration("ApiResources");
        public TableConfiguration ApiSecret { get; set; } = new TableConfiguration("ApiSecrets");
        public TableConfiguration ApiScope { get; set; } = new TableConfiguration("ApiScopes");
        public TableConfiguration ApiClaim { get; set; } = new TableConfiguration("ApiClaims");
        public TableConfiguration ApiScopeClaim { get; set; } = new TableConfiguration("ApiScopeClaims");

        public TableConfiguration Client { get; set; } = new TableConfiguration("Clients");
        public TableConfiguration ClientGrantType { get; set; } = new TableConfiguration("ClientGrantTypes");
        public TableConfiguration ClientRedirectUri { get; set; } = new TableConfiguration("ClientRedirectUris");
        public TableConfiguration ClientPostLogoutRedirectUri { get; set; } = new TableConfiguration("ClientPostLogoutRedirectUris");
        public TableConfiguration ClientScopes { get; set; } = new TableConfiguration("ClientScopes");
        public TableConfiguration ClientSecret { get; set; } = new TableConfiguration("ClientSecrets");
        public TableConfiguration ClientClaim { get; set; } = new TableConfiguration("ClientClaims");
        public TableConfiguration ClientIdPRestriction { get; set; } = new TableConfiguration("ClientIdPRestrictions");
        public TableConfiguration ClientCorsOrigin { get; set; } = new TableConfiguration("ClientCorsOrigins");
    }
}