namespace KitchenPC.Categorization.Interfaces
{
    using System.Collections.Generic;

    public interface IDBLoader
    {
        IEnumerable<IIngredientCommonality> LoadCommonIngredients();

        IEnumerable<IRecipeClassification> LoadTrainingData();
    }
}