using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer4.EntityFramework.Mappers
{
    /// <summary>
    /// AutoMapper configuration for Client
    /// Between model and entity
    /// </summary>
    public class ScopeMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="ScopeMapperProfile"/>
        /// </summary>
        public ScopeMapperProfile()
        {
            // entity to model
            CreateMap<Scope, Models.Scope>(MemberList.Destination)
                .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => x)))
                .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
            CreateMap<ScopeClaim, Models.ScopeClaim>(MemberList.Destination);
            CreateMap<ScopeSecret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

            // model to entity
            CreateMap<Models.Scope, Scope>(MemberList.Source)
                .ForMember(x => x.Claims, opts => opts.MapFrom(src => src.Claims.Select(x => x)))
                .ForMember(x => x.ScopeSecrets, opts => opts.MapFrom(src => src.ScopeSecrets.Select(x => x)));
            CreateMap<Models.ScopeClaim, ScopeClaim>(MemberList.Source);
            CreateMap<Models.Secret, ScopeSecret>(MemberList.Source);

        }
    }
}
