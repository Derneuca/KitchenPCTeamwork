namespace KitchenPC.Ingredients
{
    using System;
    using KitchenPC.ShoppingLists;

    public class IngredientAggregation : IShoppingListSource
    {
        public IngredientAggregation(Ingredient ingredient)
        {
            this.Ingredient = ingredient;

            if (ingredient != null)
            {
                this.Amount = new Amount();
                this.Amount.Unit = Unit.GetDefaultUnitType(ingredient.ConversionType);
            }
        }

        public IngredientAggregation(Ingredient ingredient, Amount baseAmount)
        {
            this.Ingredient = ingredient;
            this.Amount = baseAmount;
        }

        public Ingredient Ingredient { get; set; }

        public Amount Amount { get; set; }

        public override string ToString()
        {
            if (this.Ingredient != null && this.Amount != null)
            {
                return string.Format("{0}: {1}", this.Ingredient.Name, this.Amount);
            }

            if (this.Ingredient != null)
            {
                return this.Ingredient.Name;
            }

            return string.Empty;
        }

        public virtual IngredientAggregation AddUsage(IngredientUsage ingredient)
        {
            if (ingredient.Ingredient.Id != this.Ingredient.Id)
            {
                throw new ArgumentException("Can only call IngredientAggregation::AddUsage() on original ingredient.");
            }

            // Calculate new total
            if (this.Amount.Unit == ingredient.Amount.Unit || 
                UnitConverter.CanConvert(this.Amount.Unit, ingredient.Amount.Unit))
            {
                this.Amount += ingredient.Amount;
            }
            else 
            {
                // Find a conversion path between Ingredient and Form
                var amount = FormConversion.GetNativeAmountForUsage(this.Ingredient, ingredient);
                this.Amount += amount;
            }

            return this; // Allows AddUsage calls to be chained together
        }

        public virtual ShoppingListItem GetItem()
        {
            return new ShoppingListItem(this.Ingredient)
            {
                Amount = this.Amount
            };
        }
    }
}