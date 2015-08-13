namespace KitchenPC.Data.DTO
{
    using System;
    using KitchenPC.ShoppingLists;

    public class ShoppingLists
    {
        public Guid ShoppingListId { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; }

        public static ShoppingList ToShoppingList(ShoppingLists dtoList)
        {
            var result = new ShoppingList
            {
                Id = dtoList.ShoppingListId,
                Title = dtoList.Title
            };

            return result;
        }
    }
}