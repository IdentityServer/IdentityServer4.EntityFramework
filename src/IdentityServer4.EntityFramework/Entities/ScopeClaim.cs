namespace IdentityServer4.EntityFramework.Entities
{
    public class ScopeClaim
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AlwaysIncludeInIdToken { get; set; }
        public Scope Scope { get; set; }
    }
}