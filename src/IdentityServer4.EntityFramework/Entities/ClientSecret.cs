using System;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ClientSecret
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        public string Type { get; set; }
        public Client Client { get; set; }
    }
}