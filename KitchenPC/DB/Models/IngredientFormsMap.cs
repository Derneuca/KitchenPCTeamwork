namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class IngredientFormsMap : ClassMap<IngredientForms>
    {
        public IngredientFormsMap()
        {
            this.Id(x => x.IngredientFormId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.ConvMultiplier).Not.Nullable();
            this.Map(x => x.FormAmount).Not.Nullable();
            this.Map(x => x.UnitType).Not.Nullable();
            this.Map(x => x.UnitName).Length(50);
            this.Map(x => x.FormUnit).Not.Nullable();
            this.Map(x => x.FormDisplayName).Length(200).UniqueKey("UniqueIngredientForm");

            this.References(x => x.Ingredient).Not.Nullable().UniqueKey("UniqueIngredientForm");
        }
    }
}