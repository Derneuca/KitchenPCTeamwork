namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;
    using KitchenPC.Ingredients;

    public class IngredientForms
    {
        public virtual Guid IngredientFormId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual short ConvMultiplier { get; set; }

        public virtual float FormAmount { get; set; }

        public virtual Units UnitType { get; set; }

        public virtual string UnitName { get; set; }

        public virtual Units FormUnit { get; set; }

        public virtual string FormDisplayName { get; set; }

        public static IngredientForms FromId(Guid id)
        {
            return new IngredientForms
            {
                IngredientFormId = id
            };
        }

        public virtual IngredientForm AsIngredientForm()
        {
            return new IngredientForm
            {
                FormId = this.IngredientFormId,
                FormUnitType = this.UnitType,
                ConversionMultiplier = this.ConvMultiplier,
                FormDisplayName = this.FormDisplayName,
                FormUnitName = this.UnitName,
                IngredientId = this.Ingredient.IngredientId,
                FormAmount = new Amount(this.FormAmount, this.FormUnit)
            };
        }
    }

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