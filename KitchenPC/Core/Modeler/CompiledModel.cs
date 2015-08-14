namespace KitchenPC.Modeler
{
    using System;
    using KitchenPC.Recipes;

    /// <summary>
    /// A fully compiled result set from the modeler containing full recipe briefs and ingredient aggregation data.
    /// </summary>
    public class CompiledModel
    {
        private static CompiledModel empty;

        public static CompiledModel Empty
        {
            get
            {
                if (empty == null)
                {
                    empty = new CompiledModel()
                    {
                        Briefs = new RecipeBrief[0],
                        Pantry = new PantryItem[0],
                        RecipeIds = new Guid[0],
                        Recipes = new SuggestedRecipe[0]
                    };
                }

                return empty;
            }
        }

        public int Count
        {
            get
            {
                return this.Recipes == null ? 0 : this.Recipes.Length;
            }
        }

        public RecipeBrief[] Briefs { get; set; }

        public Guid[] RecipeIds { get; set; }

        public PantryItem[] Pantry { get; set; }

        public SuggestedRecipe[] Recipes { get; set; }
    }
}