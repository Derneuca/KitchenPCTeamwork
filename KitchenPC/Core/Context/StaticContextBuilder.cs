namespace KitchenPC.Context
{
    using System;

    public class StaticContextBuilder : IConfigurationBuilder<StaticContext>
    {
        private readonly StaticContext context;

        public StaticContextBuilder(StaticContext context)
        {
            this.context = context;
        }

        /// <summary>Configures context to compress the store file on disk to save space.</summary>
        public StaticContextBuilder CompressedStore
        {
            get
            {
                this.context.CompressedStore = true;
                return this;
            }
        }

        /// <summary>A path on the file system that contains a KitchenPC data file.</summary>
        public StaticContextBuilder DataDirectory(string path)
        {
            this.context.DataDirectory = path;
            return this;
        }

        public StaticContextBuilder Identity(Func<AuthIdentity> getIdentity)
        {
            this.context.GetIdentity = getIdentity;
            return this;
        }

        public StaticContext Create()
        {
            return this.context;
        }
    }
}