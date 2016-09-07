// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Scope = IdentityServer4.Models.Scope;
using ScopeClaim = IdentityServer4.EntityFramework.Entities.ScopeClaim;

namespace IdentityServer4.EntityFramework.Mappers
{
    public static class ScopeMappers
    {
        static ScopeMappers()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Entities.Scope, Scope>(MemberList.Destination)
                    .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<ScopeClaim, Models.ScopeClaim>(MemberList.Destination);
                config.CreateMap<ScopeSecret, Secret>(MemberList.Destination)
                    .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

                config.CreateMap<Scope, Entities.Scope>(MemberList.Source)
                    .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<Models.ScopeClaim, ScopeClaim>(MemberList.Source);
                config.CreateMap<Secret, ScopeSecret>(MemberList.Source);
            }).CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Scope ToModel(this Entities.Scope scope)
        {
            if (scope == null) return null;

            return Mapper.Map<Entities.Scope, Scope>(scope);
        }

        public static Entities.Scope ToEntity(this Scope scope)
        {
            if (scope == null) return null;

            return Mapper.Map<Scope, Entities.Scope>(scope);
        }
    }
}