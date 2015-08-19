namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class IngredientsMap : ClassMap<Ingredients>
    {
        public IngredientsMap()
        {
            this.Id(x => x.IngredientId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.FoodGroup).Length(4);
            this.Map(x => x.UsdaId).Length(5);
            this.Map(x => x.UnitName).Length(50);
            this.Map(x => x.ManufacturerName).Length(65);
            this.Map(x => x.ConversionType).Not.Nullable();
            this.Map(x => x.UnitWeight).Not.Nullable().Default("0");
            this.Map(x => x.DisplayName).Not.Nullable().Length(200).Unique().Index("IDX_Ingredients_DisplayName");
            this.Map(x => x.UsdaDesc).Length(200);

            this.HasMany(x => x.Forms).KeyColumn("IngredientId");
            this.HasOne(x => x.Metadata).PropertyRef(x => x.Ingredient).Cascade.All();
        }
    }
}