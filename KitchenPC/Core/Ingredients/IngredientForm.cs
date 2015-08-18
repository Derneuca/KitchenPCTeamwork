namespace KitchenPC.Ingredients
{
    using System;

    public class IngredientForm
    {
        public IngredientForm()
        {
        }

        public IngredientForm(Guid formId, Guid ingredientId, Units unitType, string displayName, string unitName, int conversionMultiplier, Amount amount)
        {
            this.FormId = formId;
            this.IngredientId = ingredientId;
            this.FormUnitType = unitType;
            this.FormDisplayName = displayName;
            this.FormUnitName = unitName;
            this.ConversionMultiplier = conversionMultiplier;
            this.FormAmount = amount;
        }

        public Guid FormId { get; set; }

        public Guid IngredientId { get; set; }

        public Units FormUnitType { get; set; }

        public string FormDisplayName { get; set; }

        public string FormUnitName { get; set; }

        public int ConversionMultiplier { get; set; }

        public Amount FormAmount { get; set; }

        public static IngredientForm FromId(Guid id)
        {
            return new IngredientForm
            {
                FormId = id
            };
        }

        public override string ToString()
        {
            return this.FormId.ToString();
        }
    }
}