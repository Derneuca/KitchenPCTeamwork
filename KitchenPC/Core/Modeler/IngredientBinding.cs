namespace KitchenPC.Modeler
{
    using System;

    using KitchenPC.Exceptions;
    using KitchenPC.Ingredients;

    public struct IngredientBinding
    {
        public Guid RecipeId { get; set; }

        public Guid IngredientId { get; set; }

        public float? Qty { get; set; }

        public Units Unit { get; set; }

        public static IngredientBinding Create(
            Guid ingredientId,
            Guid recipeId, 
            float? qty, 
            Units usageUnit, 
            UnitType conversionType, 
            int unitWeight,
            Units formUnit, 
            float equivAmount, 
            Units equivUnit)
        {
            var rawUnit = KitchenPC.Unit.GetDefaultUnitType(conversionType);

            if (qty.HasValue && rawUnit != usageUnit)
            {
                if (UnitConverter.CanConvert(usageUnit, rawUnit))
                {
                    qty = UnitConverter.Convert(qty.Value, usageUnit, rawUnit);
                }
                else
                {
                    var ingredient = new Ingredient
                    {
                        Id = ingredientId,
                        ConversionType = conversionType,
                        UnitWeight = unitWeight
                    };

                    var form = new IngredientForm
                    {
                        FormUnitType = formUnit,
                        FormAmount = new Amount(equivAmount, equivUnit),
                        IngredientId = ingredientId
                    };

                    var usage = new Ingredients.IngredientUsage
                    {
                        Form = form,
                        Ingredient = ingredient,
                        Amount = new Amount(qty.Value, usageUnit)
                    };

                    try
                    {
                        var newAmount = FormConversion.GetNativeAmountForUsage(ingredient, usage);
                        qty = UnitConverter.Convert(newAmount.SizeHigh, newAmount.Unit, rawUnit); // Ingredient graph only stores high amounts
                    }
                    catch (Exception e)
                    {
                        throw new DataLoadException(e);
                    }
                }
            }

            return new IngredientBinding
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Qty = qty.HasValue ? (float?)Math.Round(qty.Value, 3) : null,
                Unit = rawUnit
            };
        }
    }
}