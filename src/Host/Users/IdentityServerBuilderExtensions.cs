// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Quickstart.UI.Users;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddTestUsers(this IIdentityServerBuilder builder, List<TestUser> users = null)
        {
            builder.Services.AddSingleton(new TestUserStore(users ?? TestUsers.Users));
            builder.AddProfileService<TestUserProfileService>();
            builder.AddResourceOwnerValidator<TestUserResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}
