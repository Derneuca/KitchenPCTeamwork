using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using KitchenPC.Ingredients;

namespace KitchenPC.DB.Models
{
    public class Ingredients
    {
        public virtual Guid IngredientId { get; set; }

        public virtual string FoodGroup { get; set; }

        public virtual string UsdaId { get; set; }

        public virtual string UnitName { get; set; }

        public virtual string ManufacturerName { get; set; }

        public virtual UnitType ConversionType { get; set; }

        public virtual int UnitWeight { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string UsdaDesc { get; set; }

        public virtual IList<IngredientForms> Forms { get; set; }

        public virtual IngredientMetadata Metadata { get; set; }

        public virtual Ingredient AsIngredient()
        {
            return new Ingredient
            {
                Id = this.IngredientId,
                ConversionType = this.ConversionType,
                Name = this.DisplayName,
                UnitName = this.UnitName,
                UnitWeight = this.UnitWeight,
                Metadata = this.Metadata != null ? this.Metadata.AsIngredientMetadata() : null
            };
        }

        public static Ingredients FromId(Guid id)
        {
            return new Ingredients
            {
                IngredientId = id
            };
        }
    }

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