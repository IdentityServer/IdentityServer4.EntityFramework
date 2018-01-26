// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Xunit;
using ApiResource = IdentityServer4.Models.ApiResource;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ApiResourceMappersTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            ApiResourceMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ApiResourceMapperProfile>();
        }

        [Fact]
        public void Can_Map()
        {
            var model = new ApiResource();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }

        [Fact]
        public void Properties_Map()
        {
            var model = new ApiResource()
            {
               Description = "description",
               DisplayName = "displayname",
               Name = "foo",
               Scopes = { new Scope("foo1"), new Scope("foo2") },
               Enabled = false
            };


            var mappedEntity = model.ToEntity();

            mappedEntity.Scopes.Count.Should().Be(2);
            var foo1 = mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo1");
            foo1.Should().NotBeNull();
            var foo2 = mappedEntity.Scopes.FirstOrDefault(x => x.Name == "foo2");
            foo2.Should().NotBeNull();
            

            var mappedModel = mappedEntity.ToModel();
            
            mappedModel.Description.Should().Be("description");
            mappedModel.DisplayName.Should().Be("displayname");
            mappedModel.Enabled.Should().BeFalse();
            mappedModel.Name.Should().Be("foo");
        }
    }
}