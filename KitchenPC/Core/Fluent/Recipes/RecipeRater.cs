namespace KitchenPC.Fluent.Recipes
{
    using System.Collections.Generic;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Recipes;
    using KitchenPC.Recipes.Enums;

    /// <summary>Provides the ability to rate a recipe.</summary>
    public class RecipeRater
    {
        private readonly IKPCContext context;
        private readonly Dictionary<Recipe, Rating> newRatings;

        public RecipeRater(IKPCContext context, Recipe recipe, Rating rating)
        {
            this.context = context;
            this.newRatings = new Dictionary<Recipe, Rating>();
            this.newRatings.Add(recipe, rating);
        }

        public RecipeRater Rate(Recipe recipe, Rating rating)
        {
            this.newRatings.Add(recipe, rating);
            return this;
        }

        public void Commit()
        {
            foreach (var newRating in this.newRatings)
            {
                this.context.RateRecipe(newRating.Key.Id, newRating.Value);
            }
        }
    }
}
