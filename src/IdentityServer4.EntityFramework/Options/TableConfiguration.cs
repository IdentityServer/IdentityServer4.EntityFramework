// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.EntityFramework.Options
{
    public class TableConfiguration
    {
        public TableConfiguration(string name)
        {
            Name = name;
        }

        public TableConfiguration(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public string Name { get; set; }
        public string Schema { get; set; }
    }
}