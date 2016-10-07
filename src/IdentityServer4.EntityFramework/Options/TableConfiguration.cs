namespace IdentityServer4.EntityFramework.Options
{
    public class TableConfiguration
    {
        public TableConfiguration(string name)
        {
            Name = name;
        }

        public TableConfiguration(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public string Name { get; set; }
        public string Schema { get; set; }
    }
}