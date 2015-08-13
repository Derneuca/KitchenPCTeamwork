namespace KitchenPC.Fluent.Recipes
{
    using System;

    using KitchenPC.Context;
    using KitchenPC.Recipes;
    using KitchenPC.Recipes.Enums;

    /// <summary>Provides the ability to fluently express recipe related actions, such as loading recipes, finding recipes and sharing recipes.</summary>
    public class RecipeAction
    {
        private readonly IKPCContext context;

        public RecipeAction(IKPCContext context)
        {
            this.context = context;
        }

        public RecipeCreator Create
        {
            get
            {
                return new RecipeCreator(this.context);
            }
        }

        public RecipeLoader Load(Recipe recipe)
        {
            var result = new RecipeLoader(this.context, recipe);
            return result;
        }

        public RecipeRater Rate(Recipe recipe, Rating rating)
        {
            var result = new RecipeRater(this.context, recipe, rating);
            return result;
        }

        public RecipeFinder Search(RecipeQuery query)
        {
            var result = new RecipeFinder(this.context, query);
            return result;
        }

        public RecipeFinder Search(Func<RecipeQueryBuilder, RecipeQueryBuilder> searchBuilder)
        {
            var builder = new RecipeQueryBuilder(new RecipeQuery());
            var query = searchBuilder(builder).Query;
            var result = this.Search(query);
            return result;
        }
    }
}
