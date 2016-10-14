using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Options
{
    public class OperationalStoreOptions
    {
        public class TokenCleanupOptions
        {
            public bool Enabled { get; set; } = true;
            public int Interval { get; set; } = 60;
            public ILoggerFactory LoggerFactory { get; set; }
        }

        public string DefaultSchema { get; set; } = null;

        public TokenCleanupOptions TokenCleanup { get; set; } = new TokenCleanupOptions();

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");
    }
}