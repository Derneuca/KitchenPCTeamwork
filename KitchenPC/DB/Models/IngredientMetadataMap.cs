namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class IngredientMetadataMap : ClassMap<IngredientMetadata>
    {
        public IngredientMetadataMap()
        {
            this.Id(x => x.IngredientMetadataId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.HasMeat);
            this.Map(x => x.CarbsPerUnit);
            this.Map(x => x.HasRedMeat);
            this.Map(x => x.SugarPerUnit);
            this.Map(x => x.HasPork);
            this.Map(x => x.FatPerUnit);
            this.Map(x => x.SodiumPerUnit);
            this.Map(x => x.CaloriesPerUnit);
            this.Map(x => x.Spicy).Not.Nullable();
            this.Map(x => x.Sweet).Not.Nullable();
            this.Map(x => x.HasGluten);
            this.Map(x => x.HasAnimal);

            this.References(x => x.Ingredient).Not.Nullable().Unique();
        }
    }
}