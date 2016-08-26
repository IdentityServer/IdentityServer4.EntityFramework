using System;
using IdentityServer4.Models;

namespace IdentityServer4.EntityFramework.Mappers
{
    public static class PersistedGrantMappers
    {
        public static PersistedGrant ToModel(this Entities.PersistedGrant token)
        {
            throw new NotImplementedException();
        }

        public static Entities.PersistedGrant ToEntity(this PersistedGrant token)
        {
            throw new NotImplementedException();
        }
    }
}