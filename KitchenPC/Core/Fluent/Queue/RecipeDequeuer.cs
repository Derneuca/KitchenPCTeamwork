namespace KitchenPC.Fluent.Queue
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to dequeue one or more recipes.</summary>
    public class RecipeDequeuer
    {
        private readonly IKPCContext context;
        private readonly IList<Recipe> toDequeue;
        private bool dequeueAll;

        public RecipeDequeuer(IKPCContext context)
        {
            this.context = context;
            this.toDequeue = new List<Recipe>();
        }

        public RecipeDequeuer All
        {
            get
            {
                return new RecipeDequeuer(this.context)
                {
                    dequeueAll = true
                };
            }
        }

        public RecipeDequeuer Recipe(Recipe recipe)
        {
            this.toDequeue.Add(recipe);
            return this;
        }

        public void Commit()
        {
            if (this.dequeueAll)
            {
                this.context.DequeueRecipe();
            }
            else
            {
                this.context.DequeueRecipe(this.toDequeue.Select(r => r.Id).ToArray());
            }
        }
    }
}
