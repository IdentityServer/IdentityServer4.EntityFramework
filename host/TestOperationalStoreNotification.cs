using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.Entities;

namespace Host
{
    public class TestOperationalStoreNotification : IOperationalStoreNotification
    {
        public TestOperationalStoreNotification()
        {
            Console.WriteLine("ctor");
        }

        public Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants)
        {
            foreach (var grant in persistedGrants)
            {
                Console.WriteLine("cleaned: " + grant.Type);
            }
            return Task.CompletedTask;
        }
    }
}
