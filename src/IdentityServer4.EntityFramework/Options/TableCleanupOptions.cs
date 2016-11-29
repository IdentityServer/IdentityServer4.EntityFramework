// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.EntityFramework.Options
{
    public class TokenCleanupOptions
    {
        public int Interval { get; set; } = 60;
    }
}