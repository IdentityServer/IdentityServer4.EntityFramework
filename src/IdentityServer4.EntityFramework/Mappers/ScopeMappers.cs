// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.Mappers
{
    public static class ScopeMappers
    {
        static ScopeMappers()
        {
            Mapper = new MapperConfiguration(config =>
            {
                // entity to model
                config.CreateMap<Scope, Models.Scope>(MemberList.Destination)
                    .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<ScopeClaim, Models.ScopeClaim>(MemberList.Destination);
                config.CreateMap<ScopeSecret,Models.Secret>(MemberList.Destination)
                    .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

                // model to entity
                config.CreateMap<Models.Scope, Scope>(MemberList.Source)
                    .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
                    .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
                config.CreateMap<Models.ScopeClaim, ScopeClaim>(MemberList.Source);
                config.CreateMap<Models.Secret, ScopeSecret>(MemberList.Source);
            }).CreateMapper();
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