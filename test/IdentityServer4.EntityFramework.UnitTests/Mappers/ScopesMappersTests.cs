using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ScopesMappersTests
    {
        [Fact]
        public void ScopeAutomapperConfigurationIsValid()
        {
            var model = new Scope();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
            ScopeMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}