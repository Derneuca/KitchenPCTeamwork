namespace KitchenPC.Categorization
{
    using System.Collections.Generic;

    public interface IDBLoader
    {
        IEnumerable<IIngredientCommonality> LoadCommonIngredients();

        IEnumerable<IRecipeClassification> LoadTrainingData();
    }
}