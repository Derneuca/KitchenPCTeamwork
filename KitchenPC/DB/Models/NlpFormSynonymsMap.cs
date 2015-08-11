namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class NlpFormSynonymsMap : ClassMap<NlpFormSynonyms>
    {
        public NlpFormSynonymsMap()
        {
            this.Id(x => x.FormSynonymId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.Name).Length(50).UniqueKey("FormName");

            this.References(x => x.Ingredient).Not.Nullable().UniqueKey("FormName");
            this.References(x => x.Form).Not.Nullable();
        }
    }
}