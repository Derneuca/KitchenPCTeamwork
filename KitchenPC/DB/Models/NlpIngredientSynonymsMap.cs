namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class NlpIngredientSynonymsMap : ClassMap<NlpIngredientSynonyms>
    {
        public NlpIngredientSynonymsMap()
        {
            this.Id(x => x.IngredientSynonymId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.Alias).Length(200).Unique();
            this.Map(x => x.Prepnote).Length(50);

            this.References(x => x.Ingredient).Not.Nullable();
        }
    }
}