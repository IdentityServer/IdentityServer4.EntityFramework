// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI.Users
{
    public class TestUserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly TestUserStore _users;

        public TestUserResourceOwnerPasswordValidator(TestUserStore users)
        {
            _users = users;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_users.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _users.FindByUsername(context.UserName);
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password, user.Claims);
            }

            return Task.FromResult(0);
        }
    }
}