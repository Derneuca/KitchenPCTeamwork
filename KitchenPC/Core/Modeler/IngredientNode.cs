namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using KitchenPC.Recipes;

    public sealed class IngredientNode
    {
        private static int nextKey = 0;

        public IngredientNode()
        {
            this.Key = NextKey++;
        }

        public static int NextKey 
        { 
            get
            {
                return nextKey;
            }

            set
            {
                nextKey = value;
            }
        }

        // Interally, ingredients will have numeric keys for faster hashing
        public int Key { get; set; }

        // KPC Shopping Ingredient ID
        public Guid IngredientId { get; set; }

        // Conversion type for this ingredient
        public UnitType ConversionType { get; set; }

        // Recipes that use this ingredient (does not include Hidden recipes)
        public IEnumerable<RecipeNode>[] RecipesByTag { get; set; }

        // Which indices in RecipesByTag are not null
        public RecipeTags AvailableTags { get; set; }

        public override int GetHashCode()
        {
            return this.Key;
        }
    }
}