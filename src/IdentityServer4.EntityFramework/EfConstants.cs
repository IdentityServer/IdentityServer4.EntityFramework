namespace IdentityServer4.EntityFramework
{
    public class EfConstants
    {
        public class TableNames
        {
            public const string Scope = "Scopes";
            public const string ScopeClaim = "ScopeClaims";
            public const string ScopeSecrets = "ScopeSecrets";

            public const string PersistedGrant = "PersistedGrants";
            
            public const string Client = "Clients";
            public const string ClientGrantType = "ClientGrantTypes";
            public const string ClientRedirectUri = "ClientRedirectUris";
            public const string ClientPostLogoutRedirectUri = "ClientPostLogoutRedirectUris";
            public const string ClientScopes = "ClientScopes";
            public const string ClientSecret = "ClientSecrets";
            public const string ClientClaim = "ClientClaims";
            public const string ClientIdPRestriction = "ClientIdPRestrictions";
            public const string ClientCorsOrigin = "ClientCorsOrigins";
        }
    }
}