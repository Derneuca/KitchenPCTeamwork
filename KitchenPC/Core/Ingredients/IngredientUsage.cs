namespace KitchenPC.Ingredients
{
    using System;

    using KitchenPC.Fluent.Recipes;

    public class IngredientUsage
    {
        public IngredientUsage(Ingredient ingredient, IngredientForm form, Amount amount, string preparationNote)
        {
            this.Ingredient = ingredient;
            this.Form = form;
            this.Amount = amount;
            this.PreparationNote = preparationNote;
        }

        public IngredientUsage()
        {
        }

        public static IngredientUsageCreator Create
        {
            get
            {
                return new IngredientUsageCreator(new IngredientUsage());
            }
        }

        public Ingredient Ingredient { get; set; }

        public IngredientForm Form { get; set; }

        public Amount Amount { get; set; }

        public string PreparationNote { get; set; }

        public string Section { get; set; }

        /// <summary>Renders Ingredient Usage, using ingredientTemplate for the ingredient name.</summary>
        /// <param name="ingredientTemplate">A string template for the ingredient name, {0} will be the Ingredient Id and {1} will be the ingredient name.</param>
        /// <param name="amountTemplate">Optional string template for displaying amounts.  {0} will be numeric value, {1} will be unit.</param>
        /// <param name="multiplier">Number to multiply amount by, used to adjust recipe servings.</param>
        /// <returns>Ingredient Name (form): Amount (preparation note)</returns>
        public string ToString(string ingredientTemplate, string amountTemplate, float multiplier)
        {
            string ingredientName = 
                string.IsNullOrEmpty(ingredientTemplate) ? 
                this.Ingredient.Name :
                string.Format(ingredientTemplate, this.Ingredient.Id, this.Ingredient.Name);
            string preparation = string.Empty;
            string amount;

            if (!string.IsNullOrEmpty(this.PreparationNote))
            {
                preparation = string.Format(" ({0})", this.PreparationNote);
            }

            if (this.Amount == null) 
            {
                return string.Format("{0}{1}", ingredientName, preparation);
            }

            // Normalize amount and form
            var normalizedAmount = this.Amount == null ? null : Amount.Normalize(this.Amount, multiplier);
            if (this.Form.FormUnitType != Units.Unit && !string.IsNullOrEmpty(this.Form.FormDisplayName))
            {
                ingredientName += string.Format(" ({0})", this.Form.FormDisplayName);
            }

            var unitType = Unit.GetConvType(this.Form.FormUnitType);

            if (unitType == UnitType.Unit && !string.IsNullOrEmpty(this.Form.FormUnitName))
            {
                string[] names = this.Form.FormUnitName.Split('/');
                string unitName = (normalizedAmount.SizeLow.HasValue || normalizedAmount.SizeHigh > 1) ? names[1] : names[0];
                amount = normalizedAmount.ToString(unitName);
            }
            else
            {
                amount = normalizedAmount.ToString();
            }

            string amt = string.Format(string.IsNullOrEmpty(amountTemplate) ? "{0}{1}" : amountTemplate, amount, preparation);
            return string.Format("{0}: {1}", ingredientName, amt);
        }

        public string ToString(float multiplier)
        {
            return this.ToString(null, null, multiplier);
        }

        public override string ToString()
        {
            return this.ToString(null, null, 1);
        }
    }
}