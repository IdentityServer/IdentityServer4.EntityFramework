// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ApiScope
    {
        public int Id { get; set; }

        string _name;
        public string Name
        {
            get => _name;
            set
            {
                NormalizedName = (_name = value)?.Normalize().ToUpperInvariant();
            }
        }

        public string NormalizedName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<ApiScopeClaim> UserClaims { get; set; }

        public ApiResource ApiResource { get; set; }
    }
}