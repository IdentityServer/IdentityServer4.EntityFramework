using System.Collections.Generic;

namespace IdentityServer4.EntityFramework.Entities
{
    public class Scope
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public int Type { get; set; }
        public List<ScopeClaim> Claims { get; set; }
        public bool IncludeAllClaimsForUser { get; set; }
        public string ClaimsRule { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public List<ScopeSecret> ScopeSecrets { get; set; }
        public bool AllowUnrestrictedIntrospection { get; set; }

    }
}