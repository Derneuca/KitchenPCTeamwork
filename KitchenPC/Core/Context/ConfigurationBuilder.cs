namespace KitchenPC.Context
{
    using KitchenPC.Context.Interfaces;

    /// <summary>Fluent interface to create configuration objects</summary>
    public class ConfigurationBuilder<T> : IConfigurationBuilder<IConfiguration<T>> where T : IKPCContext
    {
        private readonly IConfiguration<T> configuration;

        public ConfigurationBuilder(IConfiguration<T> config)
        {
            this.configuration = config;
        }

        public ConfigurationBuilder<T> Context(IConfigurationBuilder<T> context)
        {
            this.configuration.Context = context.Create();
            return this;
        }

        public IConfiguration<T> Create()
        {
            return this.configuration;
        }
    }
}