// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.EntityFramework.Options
{
    /// <summary>
    /// Options for configuring the configuration context.
    /// </summary>
    public class ConfigurationStoreOptions
    {
        /// <summary>
        /// Callback to configure the EF DbContext.
        /// </summary>
        /// <value>
        /// The configure database context.
        /// </value>
        public Action<DbContextOptionsBuilder> ConfigureDbContext { get; set; }

        /// <summary>
        /// Gets or sets the default schema.
        /// </summary>
        /// <value>
        /// The default schema.
        /// </value>
        public string DefaultSchema { get; set; } = null;

        /// <summary>
        /// Gets or sets the identity resource table configuration.
        /// </summary>
        /// <value>
        /// The identity resource.
        /// </value>
        public TableConfiguration IdentityResource { get; set; } = new TableConfiguration("IdentityResources");
        /// <summary>
        /// Gets or sets the identity claim table configuration.
        /// </summary>
        /// <value>
        /// The identity claim.
        /// </value>
        public TableConfiguration IdentityClaim { get; set; } = new TableConfiguration("IdentityClaims");

        /// <summary>
        /// Gets or sets the API resource table configuration.
        /// </summary>
        /// <value>
        /// The API resource.
        /// </value>
        public TableConfiguration ApiResource { get; set; } = new TableConfiguration("ApiResources");
        /// <summary>
        /// Gets or sets the API secret table configuration.
        /// </summary>
        /// <value>
        /// The API secret.
        /// </value>
        public TableConfiguration ApiSecret { get; set; } = new TableConfiguration("ApiSecrets");
        /// <summary>
        /// Gets or sets the API scope table configuration.
        /// </summary>
        /// <value>
        /// The API scope.
        /// </value>
        public TableConfiguration ApiScope { get; set; } = new TableConfiguration("ApiScopes");
        /// <summary>
        /// Gets or sets the API claim table configuration.
        /// </summary>
        /// <value>
        /// The API claim.
        /// </value>
        public TableConfiguration ApiClaim { get; set; } = new TableConfiguration("ApiClaims");
        /// <summary>
        /// Gets or sets the API scope claim table configuration.
        /// </summary>
        /// <value>
        /// The API scope claim.
        /// </value>
        public TableConfiguration ApiScopeClaim { get; set; } = new TableConfiguration("ApiScopeClaims");

        /// <summary>
        /// Gets or sets the client table configuration.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TableConfiguration Client { get; set; } = new TableConfiguration("Clients");
        /// <summary>
        /// Gets or sets the type of the client grant table configuration.
        /// </summary>
        /// <value>
        /// The type of the client grant.
        /// </value>
        public TableConfiguration ClientGrantType { get; set; } = new TableConfiguration("ClientGrantTypes");
        /// <summary>
        /// Gets or sets the client redirect URI table configuration.
        /// </summary>
        /// <value>
        /// The client redirect URI.
        /// </value>
        public TableConfiguration ClientRedirectUri { get; set; } = new TableConfiguration("ClientRedirectUris");
        /// <summary>
        /// Gets or sets the client post logout redirect URI table configuration.
        /// </summary>
        /// <value>
        /// The client post logout redirect URI.
        /// </value>
        public TableConfiguration ClientPostLogoutRedirectUri { get; set; } = new TableConfiguration("ClientPostLogoutRedirectUris");
        /// <summary>
        /// Gets or sets the client scopes table configuration.
        /// </summary>
        /// <value>
        /// The client scopes.
        /// </value>
        public TableConfiguration ClientScopes { get; set; } = new TableConfiguration("ClientScopes");
        /// <summary>
        /// Gets or sets the client secret table configuration.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public TableConfiguration ClientSecret { get; set; } = new TableConfiguration("ClientSecrets");
        /// <summary>
        /// Gets or sets the client claim table configuration.
        /// </summary>
        /// <value>
        /// The client claim.
        /// </value>
        public TableConfiguration ClientClaim { get; set; } = new TableConfiguration("ClientClaims");
        /// <summary>
        /// Gets or sets the client IdP restriction table configuration.
        /// </summary>
        /// <value>
        /// The client IdP restriction.
        /// </value>
        public TableConfiguration ClientIdPRestriction { get; set; } = new TableConfiguration("ClientIdPRestrictions");
        /// <summary>
        /// Gets or sets the client cors origin table configuration.
        /// </summary>
        /// <value>
        /// The client cors origin.
        /// </value>
        public TableConfiguration ClientCorsOrigin { get; set; } = new TableConfiguration("ClientCorsOrigins");
        /// <summary>
        /// Gets or sets the client property table configuration.
        /// </summary>
        /// <value>
        /// The client property.
        /// </value>
        public TableConfiguration ClientProperty { get; set; } = new TableConfiguration("ClientProperties");
    }
}