// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4.EntityFramework.Entities
{
    public class Scope
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Enabled { get; set; } = true;
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public int Type { get; set; } = (int)ScopeType.Resource;
        public List<ScopeClaim> Claims { get; set; }
        public bool IncludeAllClaimsForUser { get; set; }
        public string ClaimsRule { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<ScopeSecret> ScopeSecrets { get; set; }
        public bool AllowUnrestrictedIntrospection { get; set; }

    }
}