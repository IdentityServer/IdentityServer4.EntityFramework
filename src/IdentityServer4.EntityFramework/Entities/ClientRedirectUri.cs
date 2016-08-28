namespace IdentityServer4.EntityFramework.Entities
{
    public class ClientRedirectUri
    {
        public int Id { get; set; }
        public string RedirectUri { get; set; }
        public Client Client { get; set; }
    }
}