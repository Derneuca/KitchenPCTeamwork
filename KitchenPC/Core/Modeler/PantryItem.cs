namespace KitchenPC.Modeler
{
    using System;

    public struct PantryItem
    {
        public PantryItem(Ingredients.IngredientUsage usage)
        {
            this.IngredientId = usage.Ingredient.Id;

            // Need to convert IngredientUsage into proper Pantry form
            if (usage.Amount != null)
            {
                var toUnit = Unit.GetDefaultUnitType(usage.Ingredient.ConversionType);
                if (UnitConverter.CanConvert(usage.Form.FormUnitType, toUnit))
                {
                    // Always take high amount for pantry items
                    this.Amount = UnitConverter.Convert(usage.Amount, toUnit).SizeHigh;
                }
                else 
                {
                    // Find conversion path
                    var amount = FormConversion.GetNativeAmountForUsage(usage.Ingredient, usage);

                    // Always take high amount for pantry items
                    this.Amount = UnitConverter.Convert(amount, toUnit).SizeHigh;
                }
            }
            else
            {
                this.Amount = null;
            }
        }

        // KPC Shopping Ingredient ID
        public Guid IngredientId { get; set; }

        // Optional amount of ingredient, expressed in default units for ingredient
        public float? Amount { get; set; }
    }
}