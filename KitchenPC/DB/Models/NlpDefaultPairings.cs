namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class NlpDefaultPairings
    {
        public virtual Guid DefaultPairingId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms WeightForm { get; set; }

        public virtual IngredientForms VolumeForm { get; set; }

        public virtual IngredientForms UnitForm { get; set; }
    }

    public class NlpDefaultPairingsMap : ClassMap<NlpDefaultPairings>
    {
        public NlpDefaultPairingsMap()
        {
            this.Id(x => x.DefaultPairingId)
              .GeneratedBy.GuidComb()
              .UnsavedValue(Guid.Empty);

            this.References(x => x.Ingredient).Unique().Not.Nullable();
            this.References(x => x.WeightForm);
            this.References(x => x.VolumeForm);
            this.References(x => x.UnitForm);
        }
    }
}