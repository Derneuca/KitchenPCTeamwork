namespace KitchenPC.Fluent.Recipes
{
    using System;

    using KitchenPC.Context;
    using KitchenPC.Recipes;
    using KitchenPC.Recipes.Enums;

    /// <summary>Provides the ability to fluently build a search query.</summary>
    public class RecipeCreator
    {
        private readonly IKPCContext context;
        private readonly Recipe recipe;

        public RecipeCreator(IKPCContext context)
        {
            this.context = context;
            this.recipe = new Recipe();
            this.recipe.DateEntered = DateTime.Now;
        }

        public RecipeCreator WithTitle(string title)
        {
            this.recipe.Title = title;
            return this;
        }

        public RecipeCreator WithDescription(string desc)
        {
            this.recipe.Description = desc;
            return this;
        }

        public RecipeCreator WithCredit(string credit)
        {
            this.recipe.Credit = credit;
            return this;
        }

        public RecipeCreator WithCreditUrl(Uri creditUrl)
        {
            this.recipe.CreditUrl = creditUrl.ToString();
            return this;
        }

        public RecipeCreator WithMethod(string method)
        {
            this.recipe.Method = method;
            return this;
        }

        public RecipeCreator WithDateEntered(DateTime date)
        {
            this.recipe.DateEntered = date;
            return this;
        }

        public RecipeCreator WithPrepTime(short prepTime)
        {
            this.recipe.PreparationTime = prepTime;
            return this;
        }

        public RecipeCreator WithCookTime(short cookTime)
        {
            this.recipe.CookTime = cookTime;
            return this;
        }

        public RecipeCreator WithRating(Rating rating)
        {
            this.recipe.AvgRating = (short)rating;
            return this;
        }

        public RecipeCreator WithServingSize(short servings)
        {
            this.recipe.ServingSize = servings;
            return this;
        }

        public RecipeCreator WithTags(RecipeTags tags)
        {
            this.recipe.Tags = tags;
            return this;
        }

        public RecipeCreator WithImage(Uri imageUri)
        {
            this.recipe.ImageUrl = imageUri.ToString();
            return this;
        }

        public RecipeCreator WithIngredients(Func<IngredientAdder, IngredientAdder> ingredientAdder)
        {
            var adder = ingredientAdder(new IngredientAdder(this.context, this.recipe));
            return this;
        }

        public RecipeCreator WithIngredients(string section, Func<IngredientAdder, IngredientAdder> ingredientAdder)
        {
            var adder = ingredientAdder(new IngredientAdder(this.context, this.recipe, section));
            return this;
        }

        public RecipeResult Commit()
        {
            var result = this.context.CreateRecipe(this.recipe);
            return result;
        }
    }
}
