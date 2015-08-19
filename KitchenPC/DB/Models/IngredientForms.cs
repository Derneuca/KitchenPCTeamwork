namespace KitchenPC.DB.Models
{
    using System;

    using KitchenPC.Ingredients;

    public class IngredientForms
    {
        public virtual Guid IngredientFormId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual short ConvMultiplier { get; set; }

        public virtual float FormAmount { get; set; }

        public virtual Units UnitType { get; set; }

        public virtual string UnitName { get; set; }

        public virtual Units FormUnit { get; set; }

        public virtual string FormDisplayName { get; set; }

        public static IngredientForms FromId(Guid id)
        {
            return new IngredientForms
            {
                IngredientFormId = id
            };
        }

        public virtual IngredientForm AsIngredientForm()
        {
            return new IngredientForm
            {
                FormId = this.IngredientFormId,
                FormUnitType = this.UnitType,
                ConversionMultiplier = this.ConvMultiplier,
                FormDisplayName = this.FormDisplayName,
                FormUnitName = this.UnitName,
                IngredientId = this.Ingredient.IngredientId,
                FormAmount = new Amount(this.FormAmount, this.FormUnit)
            };
        }
    }
}