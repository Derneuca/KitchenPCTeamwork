namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class NlpUnitSynonymsMap : ClassMap<NlpUnitSynonyms>
    {
        public NlpUnitSynonymsMap()
        {
            this.Id(x => x.UnitSynonymId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.Name).Length(50).UniqueKey("UniquePair");

            this.References(x => x.Ingredient).Not.Nullable().UniqueKey("UniquePair");
            this.References(x => x.Form).Not.Nullable();
        }
    }
}