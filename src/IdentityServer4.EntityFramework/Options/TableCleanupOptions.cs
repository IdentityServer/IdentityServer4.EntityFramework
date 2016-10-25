namespace IdentityServer4.EntityFramework.Options
{
    public class TokenCleanupOptions
    {
        public int Interval { get; set; } = 60;
    }
}