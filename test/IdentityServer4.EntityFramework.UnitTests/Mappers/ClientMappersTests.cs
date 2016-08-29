using System.Collections.Generic;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ClientMappersTests
    {
        [Fact]
        public void ClientAutomapperConfigurationIsValid()
        {
            var model = new Client();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();
            
            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
            ClientMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}