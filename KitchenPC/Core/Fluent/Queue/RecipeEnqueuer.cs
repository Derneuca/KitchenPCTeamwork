namespace KitchenPC.Fluent.Queue
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to enqueue one or more recipes.</summary>
    public class RecipeEnqueuer
    {
        private readonly IKPCContext context;
        private readonly IList<Recipe> recipesQueue;

        public RecipeEnqueuer(IKPCContext context)
        {
            this.context = context;
            this.recipesQueue = new List<Recipe>();
        }

        public RecipeEnqueuer Recipe(Recipe recipe)
        {
            this.recipesQueue.Add(recipe);
            return this;
        }

        public void Commit()
        {
            if (this.recipesQueue.Any())
            {
                this.context.EnqueueRecipes(this.recipesQueue.Select(r => r.Id).ToArray());
            }
        }
    }
}
