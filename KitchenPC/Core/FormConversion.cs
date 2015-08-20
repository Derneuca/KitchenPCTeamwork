namespace KitchenPC
{
    using System;

    using KitchenPC.Exceptions;
    using KitchenPC.Ingredients;

    public static class FormConversion
    {
        public static Amount GetWeightForUsage(IngredientUsage usage)
        {
            if (Unit.GetConvertionType(usage.Form.FormUnitType) == UnitType.Weight)
            {
                // Already there, just convert to grams
                return UnitConverter.Convert(usage.Amount, Units.Gram);
            }

            if (usage.Ingredient.ConversionType == UnitType.Weight) 
            {
                // Ingredient is sold in weight, so we can use its native amount
                var amount = GetNativeAmountForUsage(usage.Ingredient, usage);
                return UnitConverter.Convert(amount, Units.Gram);
            }

            if (usage.Ingredient.ConversionType == UnitType.Unit && 
                usage.Ingredient.UnitWeight > 0) 
            {
                // Ingredient sold in units, but we know weight of each
                var amount = GetNativeAmountForUsage(usage.Ingredient, usage);
                amount.Unit = Units.Gram;
                amount *= usage.Ingredient.UnitWeight;

                return amount;
            }

            if (Unit.GetConvertionType(usage.Form.FormAmount.Unit) == UnitType.Weight && 
                usage.Form.FormAmount.SizeHigh > 0) 
            {
                // This form has a gram weight
                var amount = UnitConverter.Convert(usage.Amount, usage.Form.FormUnitType);
                return new Amount(amount.SizeHigh * usage.Form.FormAmount.SizeHigh, Units.Gram);
            }

            return null;
        }

        public static Amount GetNativeAmountForUsage(Ingredient ingredient, IngredientUsage usage)
        {
            var amount = new Amount();
            var usageConvertionType = Unit.GetConvertionType(usage.Form.FormUnitType);

            switch (ingredient.ConversionType)
            {
                case UnitType.Unit:
                    {
                        amount.Unit = Units.Unit;
                        switch (usageConvertionType)
                        {
                            case UnitType.Unit:
                                {
                                    // Unit to unit version
                                    var equivalentGrams = UnitConverter.Convert(usage.Form.FormAmount, Units.Gram);
                                    amount.SizeHigh = 
                                        (float)Math.Ceiling((equivalentGrams.SizeHigh * usage.Amount.SizeHigh) / 
                                        ingredient.UnitWeight);
                                    return amount;
                                }

                            case UnitType.Volume:
                                {
                                    // Volume to unit conversion
                                    var likeAmount = UnitConverter.Convert(usage.Amount, usage.Form.FormUnitType);
                                    amount.SizeHigh = 
                                        (float)Math.Ceiling((likeAmount.SizeHigh * usage.Form.FormAmount.SizeHigh) / 
                                        usage.Ingredient.UnitWeight);
                                    return amount;
                                }

                            case UnitType.Weight:
                                {
                                    // Weight to unit conversion
                                    var grams = UnitConverter.Convert(usage.Amount, Units.Gram);
                                    amount.SizeHigh = 
                                        (float)Math.Ceiling(grams.SizeHigh / 
                                        ingredient.UnitWeight);
                                    return amount;
                                }
                        }

                        break;
                    }
                case UnitType.Weight:
                    {
                        amount.Unit = Units.Gram;
                        switch (usageConvertionType)
                        {
                            case UnitType.Unit:
                                {
                                    // Unit to weight conversion
                                    amount.SizeHigh = usage.Amount.SizeHigh * usage.Form.FormAmount.SizeHigh;
                                    return amount;
                                }

                            case UnitType.Volume:
                                {
                                    // Volume to weight conversion
                                    var likeAmount = UnitConverter.Convert(usage.Amount, usage.Form.FormUnitType);
                                    amount.SizeHigh = likeAmount.SizeHigh * usage.Form.FormAmount.SizeHigh;
                                    return amount;
                                }
                        }

                        break;
                    }
                case UnitType.Volume:
                    {
                        amount.Unit = Units.Teaspoon;
                        switch (usageConvertionType)
                        {
                            case UnitType.Unit:
                                {
                                    // Unit to volume conversion
                                    amount.SizeHigh = usage.Amount.SizeHigh * usage.Form.FormAmount.SizeHigh;
                                    return amount;
                                }

                            case UnitType.Weight:
                                {
                                    // Weight to volume conversion
                                    var likeAmount = UnitConverter.Convert(usage.Amount, usage.Form.FormUnitType);
                                    amount.SizeHigh = likeAmount.SizeHigh * usage.Form.FormAmount.SizeHigh;
                                    return amount;
                                }
                        }

                        break;
                    }
            }

            throw new IngredientAggregationDatabaseException(string.Format("Cannot convert an IngredientUsage into its native form.", ingredient, usage));
        }
    }
}