namespace KitchenPC.Modeler
{
    using System.Collections.Generic;

    public interface IModelerLoader
    {
        IEnumerable<RecipeBinding> LoadRecipeGraph();

        IEnumerable<IngredientBinding> LoadIngredientGraph();

        IEnumerable<RatingBinding> LoadRatingGraph();
    }
}