namespace IdentityServer4.EntityFramework.Options
{
    public class OperationalStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");
    }
}