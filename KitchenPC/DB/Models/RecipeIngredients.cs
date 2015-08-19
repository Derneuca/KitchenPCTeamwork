namespace KitchenPC.DB.Models
{
    using System;

    public class RecipeIngredients
    {
        public virtual Guid RecipeIngredientId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms IngredientForm { get; set; }

        public virtual Units Unit { get; set; }

        public virtual float? QtyLow { get; set; }

        public virtual short DisplayOrder { get; set; }

        public virtual string PrepNote { get; set; }

        public virtual float? Qty { get; set; }

        public virtual string Section { get; set; }
    }
}