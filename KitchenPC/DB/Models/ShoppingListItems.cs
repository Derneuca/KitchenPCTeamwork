namespace KitchenPC.DB.Models
{
    using System;

    using KitchenPC.ShoppingLists;

    public sealed class ShoppingListItems
    {
        public ShoppingListItems()
        {
        }

        public ShoppingListItems(Guid id, Guid userid, string raw)
        {
            this.ItemId = id;
            this.UserId = userid;
            this.Raw = raw;
        }

        public ShoppingListItems(Guid id, Guid userid, Amount amt, Guid? ingredientId, Guid? recipeId)
        {
            this.ItemId = id;
            this.UserId = userid;
            this.Amount = amt;
            this.Ingredient = ingredientId.HasValue ? Ingredients.FromId(ingredientId.Value) : null;
            this.Recipe = recipeId.HasValue ? Recipes.FromId(recipeId.Value) : null;
        }

        public Guid ItemId { get; set; }

        public string Raw { get; set; }

        public float? Qty { get; set; }

        public Units? Unit { get; set; }

        public Guid UserId { get; set; }

        public Ingredients Ingredient { get; set; }

        public Recipes Recipe { get; set; }

        public ShoppingLists ShoppingList { get; set; }

        public bool CrossedOut { get; set; }

        public Amount Amount
        {
            get
            {
                return this.Qty.HasValue && this.Unit.HasValue ? new Amount(this.Qty.Value, this.Unit.Value) : null;
            }

            set
            {
                if (value == null || value.SizeHigh == 0)
                {
                    this.Qty = null;
                    this.Unit = null;
                }
                else
                {
                    this.Qty = value.SizeHigh;
                    this.Unit = value.Unit;
                }
            }
        }

        public static ShoppingListItems FromShoppingListItem(ShoppingListItem item)
        {
            return new ShoppingListItems
            {
                ItemId = item.Id.HasValue ? item.Id.Value : Guid.NewGuid(),
                Raw = item.Raw,
                Qty = item.Amount == null ? null : (float?)item.Amount.SizeHigh,
                Unit = item.Amount == null ? null : (Units?)item.Amount.Unit,
                Ingredient = item.Ingredient == null ? null : new Ingredients { IngredientId = item.Ingredient.Id },
                Recipe = item.Recipe == null ? null : new Recipes { RecipeId = item.Recipe.Id },
                CrossedOut = item.CrossedOut
            };
        }

        public ShoppingListItem AsShoppingListItem()
        {
            if (this.Ingredient != null)
            {
                return new ShoppingListItem(this.Ingredient.AsIngredient())
                {
                    Id = this.ItemId,
                    Amount = (this.Qty.HasValue && this.Unit.HasValue) ? new Amount(this.Qty.Value, this.Unit.Value) : null,
                    Recipe = this.Recipe != null ? this.Recipe.AsRecipeBrief() : null,
                    CrossedOut = this.CrossedOut
                };
            }

            return new ShoppingListItem(this.Raw)
            {
                Id = this.ItemId,
                CrossedOut = this.CrossedOut
            };
        }
    }
}