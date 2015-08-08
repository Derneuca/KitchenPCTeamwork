namespace KitchenPC.Recipes
{
    using System;

    public class RecipeResult
    {
        public bool RecipeCreated { get; set; }

        public Guid? NewRecipeId { get; set; }
    }
}