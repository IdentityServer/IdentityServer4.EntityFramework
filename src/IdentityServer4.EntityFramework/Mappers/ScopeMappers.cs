// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.Mappers
{
    public static class ScopeMappers
    {
        static ScopeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ScopeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Models.Scope ToModel(this Scope scope)
        {
            return scope == null ? null : Mapper.Map<Models.Scope>(scope);
        }

        public static Scope ToEntity(this Models.Scope scope)
        {
            return scope == null ? null : Mapper.Map<Scope>(scope);
        }
    }
}