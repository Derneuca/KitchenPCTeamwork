namespace KitchenPC.Context
{
    using System.Collections.Generic;
    using KitchenPC.Modeler;

    public class DBModelerLoader : IModelerLoader
    {
        private readonly IDBAdapter adapter;
        private IEnumerable<RecipeBinding> recipeData;
        private IEnumerable<IngredientBinding> ingredientData;
        private IEnumerable<RatingBinding> ratingData;

        public DBModelerLoader(IDBAdapter adapter)
        {
            this.adapter = adapter;
        }

        public IEnumerable<RecipeBinding> LoadRecipeGraph()
        {
            if (this.recipeData == null)
            {
                this.recipeData = this.adapter.LoadRecipeGraph();
            }

            return this.recipeData;
        }

        public IEnumerable<IngredientBinding> LoadIngredientGraph()
        {
            if (this.ingredientData == null)
            {
                this.ingredientData = this.adapter.LoadIngredientGraph();
            }

            return this.ingredientData;
        }

        public IEnumerable<RatingBinding> LoadRatingGraph()
        {
            if (this.ratingData == null)
            {
                this.ratingData = this.adapter.LoadRatingGraph();
            }

            return this.ratingData;
        }
    }
}