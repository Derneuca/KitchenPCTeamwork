namespace KitchenPC.ShoppingLists
{
    using System;

    public class ShoppingListResult
    {
        public Guid? NewShoppingListId { get; set; }

        public ShoppingList List { get; set; }
    }
}