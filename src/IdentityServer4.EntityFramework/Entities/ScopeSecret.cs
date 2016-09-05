using System;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ScopeSecret
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
        public Scope Scope { get; set; }
    }
}