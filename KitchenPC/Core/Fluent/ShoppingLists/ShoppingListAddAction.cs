namespace KitchenPC.Fluent.ShoppingLists
{
    using KitchenPC.Ingredients;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to fluently express a ShoppingListAdder object.</summary>
    public class ShoppingListAddAction
    {
        private readonly ShoppingListAdder adder;

        public ShoppingListAddAction(ShoppingListAdder adder)
        {
            this.adder = adder;
        }

        public ShoppingListAdder Adder
        {
            get
            {
                return this.adder;
            }
        }

        public ShoppingListAddAction AddRecipe(Recipe recipe)
        {
            this.adder.Recipes.Add(recipe);
            return this;
        }

        public ShoppingListAddAction AddIngredient(Ingredient ingredient)
        {
            this.adder.Ingredients.Add(ingredient);
            return this;
        }

        public ShoppingListAddAction AddUsage(IngredientUsage usage)
        {
            this.adder.Usages.Add(usage);
            return this;
        }

        public ShoppingListAddAction AddItem(string raw)
        {
            this.adder.ToParse.Add(raw);
            return this;
        }
    }
}
