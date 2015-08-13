namespace KitchenPC.Fluent.Queue
{
    using System.Collections.Generic;

    using KitchenPC.Context;
    using KitchenPC.Recipes;

    /// <summary>Loads the recipe queue.</summary>
    public class QueueLoader
    {
        private readonly IKPCContext context;

        public QueueLoader(IKPCContext context)
        {
            this.context = context;
        }

        public IList<RecipeBrief> List()
        {
            return this.context.GetRecipeQueue();
        }
    }
}
