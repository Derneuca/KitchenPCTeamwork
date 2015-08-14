namespace KitchenPC.Modeler
{
    using System;
    using KitchenPC.Ingredients;

    /// <summary>
    /// A recipe suggested by the modeler.
    /// </summary>
    public class SuggestedRecipe
    {
        public Guid Id { get; set; }

        public IngredientAggregation[] Ingredients { get; set; }
    }
}