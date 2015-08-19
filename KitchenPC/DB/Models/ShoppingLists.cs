namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;

    using KitchenPC.ShoppingLists;

    public class ShoppingLists
    {
        public virtual Guid ShoppingListId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual string Title { get; set; }

        public virtual IList<ShoppingListItems> Items { get; set; }

        public static ShoppingLists FromId(Guid id)
        {
            return new ShoppingLists
            {
                ShoppingListId = id
            };
        }

        public virtual ShoppingList AsShoppingList()
        {
            return new ShoppingList
            {
                Id = this.ShoppingListId,
                Title = this.Title
            };
        }
    }
}