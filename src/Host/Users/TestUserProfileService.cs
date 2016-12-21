// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI.Users
{
    public class TestUserProfileService : IProfileService
    {
        private readonly TestUserStore _users;

        public TestUserProfileService(TestUserStore users)
        {
            _users = users;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = _users.FindBySubjectId(context.Subject.GetSubjectId());

            context.AddFilteredClaims(user.Claims);

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.FromResult(0);
        }
    }
}