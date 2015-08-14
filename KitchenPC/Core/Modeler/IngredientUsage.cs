namespace KitchenPC.Modeler
{
    using System;

    public struct IngredientUsage
    {
        // Reference to IngredientNode describing this ingredient
        public IngredientNode Ingredient { get; set; }

        // Amount of ingredient, expressed in default units for ingredient
        public float? Amount { get; set; }

        // Unit for this amount (will always be compatible with Ingredient.ConvType)
        public Units Unit { get; set; }
    }
}