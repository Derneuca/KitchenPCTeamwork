namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class RecipeIngredientsMap : ClassMap<RecipeIngredients>
    {
        public RecipeIngredientsMap()
        {
            this.Id(x => x.RecipeIngredientId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.Unit).Not.Nullable();
            this.Map(x => x.QtyLow);
            this.Map(x => x.DisplayOrder).Not.Nullable();
            this.Map(x => x.PrepNote).Length(50);
            this.Map(x => x.Qty);
            this.Map(x => x.Section).Length(50);

            this.References(x => x.Recipe).Column("RecipeId").Not.Nullable().Index("IDX_RecipeIngredients_RecipeId");
            this.References(x => x.Ingredient).Column("IngredientId").Not.Nullable().Index("IDX_RecipeIngredients_IngredientId");
            this.References(x => x.IngredientForm).Column("IngredientFormId");
        }
    }
}