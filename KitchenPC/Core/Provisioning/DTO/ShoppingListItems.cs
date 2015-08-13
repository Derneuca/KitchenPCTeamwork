namespace KitchenPC.Data.DTO
{
    using System;

    public class ShoppingListItems
    {
        public Guid ItemId { get; set; }

        public Guid UserId { get; set; }

        public Guid? IngredientId { get; set; }

        public Guid? RecipeId { get; set; }

        public Guid? ShoppingListId { get; set; }

        public Units? Unit { get; set; }

        public string Raw { get; set; }

        public float? Qty { get; set; }

        public bool CrossedOut { get; set; }
    }
}