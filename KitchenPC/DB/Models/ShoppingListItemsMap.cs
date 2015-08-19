namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

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