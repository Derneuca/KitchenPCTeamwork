namespace KitchenPC.Fluent.Recipes
{
    using KitchenPC.Context;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to search for recipe.</summary>
    public class RecipeFinder
    {
        private readonly IKPCContext context;
        private readonly RecipeQuery query;

        public RecipeFinder(IKPCContext context, RecipeQuery query)
        {
            this.context = context;
            this.query = query;
        }

        public SearchResults Results()
        {
            var result = this.context.RecipeSearch(this.query);
            return result;
        }
    }
}
