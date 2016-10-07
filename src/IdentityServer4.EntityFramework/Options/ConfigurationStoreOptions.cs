namespace IdentityServer4.EntityFramework.Options
{
    public class ConfigurationStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration Scope { get; set; } = new TableConfiguration("Scopes");
        public TableConfiguration ScopeClaim { get; set; } = new TableConfiguration("ScopeClaims");
        public TableConfiguration ScopeSecret { get; set; } = new TableConfiguration("ScopeSecrets");

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