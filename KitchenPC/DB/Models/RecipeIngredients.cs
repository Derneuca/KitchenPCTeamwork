namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class RecipeIngredients
    {
        public virtual Guid RecipeIngredientId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms IngredientForm { get; set; }

        public virtual Units Unit { get; set; }

        public virtual float? QtyLow { get; set; }

        public virtual short DisplayOrder { get; set; }

        public virtual string PrepNote { get; set; }

        public virtual float? Qty { get; set; }

        public virtual string Section { get; set; }
    }

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