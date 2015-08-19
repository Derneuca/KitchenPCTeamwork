namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class NlpAnomalousIngredientsMap : ClassMap<NlpAnomalousIngredients>
    {
        public NlpAnomalousIngredientsMap()
        {
            this.Id(x => x.AnomalousIngredientId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.Name).Not.Nullable().Length(100).Unique();

            this.References(x => x.Ingredient).Not.Nullable();
            this.References(x => x.WeightForm);
            this.References(x => x.VolumeForm);
            this.References(x => x.UnitForm);
        }
    }
}