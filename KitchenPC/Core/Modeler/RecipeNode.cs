namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using KitchenPC.Recipes;

    public sealed class RecipeNode
    {
        private static int nextkey = 0;

        public RecipeNode()
        {
            this.Key = nextkey++;
        }

        // Internally, recipes will have numeric keys for faster hashing
        public int Key { get; set; }

        public Guid RecipeId { get; set; }

        public IEnumerable<IngredientUsage> Ingredients { get; set; }

        // Tags from DB
        public RecipeTags Tags { get; set; }

        // Public rating from DB
        public byte Rating { get; set; }

        // Recipe is hidden (won't be picked at random)
        public bool Hidden { get; set; }

        // Users who like this recipe might also like these recipes (in order of weight)
        public RecipeNode[] Suggestions { get; set; }

        public override int GetHashCode()
        {
            return this.Key;
        }
    }
}