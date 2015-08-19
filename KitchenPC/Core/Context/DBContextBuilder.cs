namespace KitchenPC.Context
{
    using System;

    using KitchenPC.Context.Interfaces;

    public class DBContextBuilder : IConfigurationBuilder<DBContext>
    {
        private readonly DBContext context;

        public DBContextBuilder(DBContext context)
        {
            this.context = context;
        }

        public DBContextBuilder Adapter<T>(IConfigurationBuilder<T> adapter) where T : IDBAdapter
        {
            this.context.Adapter = adapter.Create();
            return this;
        }

        public DBContextBuilder Identity(Func<AuthIdentity> getIdentity)
        {
            this.context.GetIdentity = getIdentity;
            return this;
        }

        public DBContext Create()
        {
            return this.context;
        }
    }
}