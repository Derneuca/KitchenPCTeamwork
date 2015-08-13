namespace KitchenPC.Fluent.ShoppingLists
{
    using KitchenPC.Context;
    using KitchenPC.ShoppingLists;

    /// <summary>Provides the ability to fluently express shopping list related actions, such as loading, creating or updating lists.</summary>
    public class ShoppingListAction
    {
        private readonly IKPCContext context;

        public ShoppingListAction(IKPCContext context)
        {
            this.context = context;
        }

        public ShoppingListCreator Create
        {
            get
            {
                var result = new ShoppingListCreator(this.context);
                return result;
            }
        }

        public ShoppingListLoader LoadAll
        {
            get
            {
                var result = new ShoppingListLoader(this.context);
                return result;
            }
        }

        public ShoppingListLoader Load(ShoppingList list)
        {
            var result = new ShoppingListLoader(this.context, list);
            return result;
        }

        public ShoppingListDeleter Delete(ShoppingList list)
        {
            if (list == ShoppingList.Default)
            {
                throw new FluentExpressionException("Cannot delete default shopping list.");
            }

            var result = new ShoppingListDeleter(this.context, list);
            return result;
        }

        public ShoppingListUpdater Update(ShoppingList list)
        {
            var result = new ShoppingListUpdater(this.context, list);
            return result;
        }
    }
}
