namespace KitchenPC.DB
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Context.Interfaces;
    using FluentNHibernate.Cfg.Db;
    using FluentNHibernate.Conventions;

    public class DatabaseAdapterBuilder : IConfigurationBuilder<DatabaseAdapter>
    {
        private readonly DatabaseAdapter adapter;

        public DatabaseAdapterBuilder(DatabaseAdapter adapter)
        {
            this.adapter = adapter;
        }

        public DatabaseAdapter Create()
        {
            return this.adapter;
        }

        public DatabaseAdapterBuilder AddConvention(IConvention convention)
        {
            if (this.adapter.DatabaseConventions == null)
            {
                this.adapter.DatabaseConventions = new List<IConvention>();
            }

            this.adapter.DatabaseConventions.Add(convention);

            return this;
        }

        public DatabaseAdapterBuilder DatabaseConfiguration(IPersistenceConfigurer config)
        {
            this.adapter.DatabaseConfiguration = config;
            return this;
        }

        public DatabaseAdapterBuilder SearchProvider<T>(Func<DatabaseAdapter, T> createProvider)
            where T : ISearchProvider
        {
            this.adapter.SearchProvider = createProvider(this.adapter);
            return this;
        }
    }
}