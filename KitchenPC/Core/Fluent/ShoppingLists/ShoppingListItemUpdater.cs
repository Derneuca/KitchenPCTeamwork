namespace KitchenPC.Fluent.ShoppingLists
{
    using KitchenPC.ShoppingLists;

    /// <summary>Represents a set of changes to a single shopping list item.</summary>
    public class ShoppingListItemUpdater
    {
        public ShoppingListItemUpdater(ShoppingListItem item)
        {
            this.Item = item;
        }

        public ShoppingListItem Item { get; set; }

        public Amount NewAmount { get; set; }

        public bool? CrossedOut { get; set; }

        public static ShoppingListItemUpdateAction Create(ShoppingListItem item)
        {
            var updater = new ShoppingListItemUpdater(item);
            var result = new ShoppingListItemUpdateAction(updater);
            return result;
        }
    }
}
