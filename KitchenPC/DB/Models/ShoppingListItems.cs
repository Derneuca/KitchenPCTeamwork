namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;
    using KitchenPC.ShoppingLists;

    public class ShoppingListItems
    {
        public ShoppingListItems()
        {
        }

        public ShoppingListItems(Guid id, Guid userid, String raw)
        {
            ItemId = id;
            UserId = userid;
            Raw = raw;
        }

        public ShoppingListItems(Guid id, Guid userid, Amount amt, Guid? ingredientId, Guid? recipeId)
        {
            ItemId = id;
            UserId = userid;
            Amount = amt;
            Ingredient = ingredientId.HasValue ? Ingredients.FromId(ingredientId.Value) : null;
            Recipe = recipeId.HasValue ? Recipes.FromId(recipeId.Value) : null;
        }

        public virtual Guid ItemId { get; set; }

        public virtual String Raw { get; set; }

        public virtual float? Qty { get; set; }

        public virtual Units? Unit { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual ShoppingLists ShoppingList { get; set; }

        public virtual bool CrossedOut { get; set; }

        public virtual Amount Amount
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

        public virtual ShoppingListItem AsShoppingListItem()
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

    public class ShoppingListItemsMap : ClassMap<ShoppingListItems>
    {
        public ShoppingListItemsMap()
        {
            this.Id(x => x.ItemId)
               .GeneratedBy.GuidComb()
               .UnsavedValue(Guid.Empty);

            this.Map(x => x.Raw).Length(50);
            this.Map(x => x.Qty);
            this.Map(x => x.Unit);
            this.Map(x => x.UserId).Not.Nullable().Index("IDX_ShoppingListItems_UserId");
            this.Map(x => x.CrossedOut).Not.Nullable();

            this.References(x => x.Recipe).Column("RecipeId");
            this.References(x => x.Ingredient).Column("IngredientId");
            this.References(x => x.ShoppingList).Column("ShoppingListId").Index("IDX_ShoppingListItems_ListId");
        }
    }
}