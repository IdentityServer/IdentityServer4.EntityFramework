namespace IdentityServer4.EntityFramework.Entities
{
    public class ClientIdPRestriction
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public Client Client { get; set; }
    }
}