namespace KitchenPC.Fluent.ShoppingLists
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Exceptions;
    using KitchenPC.ShoppingLists;

    public class ShoppingListDeleter
    {
        private readonly IKPCContext context;
        private readonly IList<ShoppingList> deleteQueue;

        public ShoppingListDeleter(IKPCContext context, ShoppingList list)
        {
            this.context = context;
            this.deleteQueue = new List<ShoppingList>() { list };
        }

        public ShoppingListDeleter Delete(ShoppingList list)
        {
            if (list == ShoppingList.Default)
            {
                throw new FluentExpressionException("Cannot delete default shopping list.");
            }

            this.deleteQueue.Add(list);
            return this;
        }

        public void Commit()
        {
            this.context.DeleteShoppingLists(this.deleteQueue.ToArray());
        }
    }
}
