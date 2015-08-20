namespace KitchenPC.Fluent.ShoppingLists
{
    using System.Collections.Generic;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Exceptions;
    using KitchenPC.ShoppingLists;

    public class ShoppingListLoader
    {
        private readonly IKPCContext context;
        private readonly IList<ShoppingList> listsToLoad;
        private readonly bool loadAll;
        private bool loadItems;

        public ShoppingListLoader(IKPCContext context)
        {
            this.context = context;
            this.loadAll = true;
        }

        public ShoppingListLoader(IKPCContext context, ShoppingList list)
        {
            this.context = context;
            this.listsToLoad = new List<ShoppingList>() { list };
        }

        public ShoppingListLoader WithItems
        {
            get
            {
                this.loadItems = true;
                return this;
            }
        }

        public ShoppingListLoader Load(ShoppingList list)
        {
            if (this.loadAll)
            {
                throw new FluentExpressionException("To specify individual shopping lists to load, remove the LoadAll clause from your expression.");
            }

            this.listsToLoad.Add(list);
            return this;
        }

        public IList<ShoppingList> List()
        {
            var options = new GetShoppingListOptions(this.loadItems);
            var result = this.context.GetShoppingLists(this.listsToLoad, options);
            return result;
        }
    }
}
