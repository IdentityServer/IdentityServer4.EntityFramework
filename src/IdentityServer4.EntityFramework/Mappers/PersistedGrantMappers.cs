using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.EntityFramework.Mappers
{
    public static class PersistedGrantMappers
    {
        static PersistedGrantMappers()
        {
            Mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Entities.PersistedGrant, PersistedGrant>(MemberList.Destination);
                config.CreateMap<PersistedGrant, Entities.PersistedGrant>(MemberList.Source);
            }).CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static PersistedGrant ToModel(this Entities.PersistedGrant token)
        {
            if (token == null) return null;

            return Mapper.Map<Entities.PersistedGrant, PersistedGrant>(token);
        }

        public static Entities.PersistedGrant ToEntity(this PersistedGrant token)
        {
            if (token == null) return null;

            return Mapper.Map<PersistedGrant, Entities.PersistedGrant>(token);
        }
    }
}