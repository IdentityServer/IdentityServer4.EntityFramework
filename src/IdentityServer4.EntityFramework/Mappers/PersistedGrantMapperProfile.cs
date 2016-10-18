using AutoMapper;

namespace IdentityServer4.EntityFramework.Mappers
{
    /// <summary>
    /// AutoMapper Config for PersistedGrant
    /// Between Model and Entity
    /// <seealso cref="https://github.com/AutoMapper/AutoMapper/wiki/Configuration">
    /// </seealso>
    /// </summary>
    public class PersistedGrantMapperProfile:Profile
    {
        /// <summary>
        /// <see cref="PersistedGrantMapperProfile">
        /// </see>
        /// </summary>
        public PersistedGrantMapperProfile()
        {
            // entity to model
            CreateMap<Entities.PersistedGrant, Models.PersistedGrant>(MemberList.Destination);

            // model to entity
            CreateMap<Models.PersistedGrant, Entities.PersistedGrant>(MemberList.Source);
        }
    }
}
