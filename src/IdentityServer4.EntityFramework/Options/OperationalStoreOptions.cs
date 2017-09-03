// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.EntityFramework.Options
{
    public class OperationalStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");

        public bool EnableTokenCleanup { get; set; } = false;
        public int TokenCleanupInterval { get; set; } = 3600;
    }
}