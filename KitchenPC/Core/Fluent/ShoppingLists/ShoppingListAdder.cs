namespace KitchenPC.Fluent.ShoppingLists
{
    using System.Collections.Generic;

    using KitchenPC.Ingredients;
    using KitchenPC.Recipes;

    /// <summary>Represents one or more items to be added to a shopping list.</summary>
    public class ShoppingListAdder
    {
        public ShoppingListAdder()
        {
            this.Recipes = new List<Recipe>();
            this.Ingredients = new List<Ingredient>();
            this.Usages = new List<IngredientUsage>();
            this.ToParse = new List<string>();
        }

        // Recipes to add
        public IList<Recipe> Recipes { get; set; }

        // Resolved ingredients to add
        public IList<Ingredient> Ingredients { get; set; }

        // Ingredient Usages which will be aggregated upon save
        public IList<IngredientUsage> Usages { get; set; }

        // Raw ingredient strings that will be parsed, or added in raw form
        public IList<string> ToParse { get; set; }

        public static ShoppingListAddAction Create()
        {
            var adder = new ShoppingListAdder();
            var result = new ShoppingListAddAction(adder);
            return result;
        }
    }
}
