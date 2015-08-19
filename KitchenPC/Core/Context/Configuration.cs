namespace KitchenPC.Context
{
    using System;

    public class Configuration<T> : IConfiguration<T> where T : IKPCContext
    {
        private readonly ConfigurationBuilder<T> builder;

        private Configuration()
        {
            this.builder = new ConfigurationBuilder<T>(this);
        }

        public static ConfigurationBuilder<T> Build
        {
            get
            {
                return new Configuration<T>().builder;
            }
        }

        public static IConfiguration<T> Xml
        {
            get
            {
                // TODO: Read local XML configuration and return ConfigurationBuilder
                throw new NotImplementedException();
            }
        }

        public T Context { get; set; }

        public T InitializeContext()
        {
            this.Context.Initialize();
            return this.Context;
        }
    }
}