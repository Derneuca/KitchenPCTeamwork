namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;

    using KitchenPC.Ingredients;

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

        public static Ingredients FromId(Guid id)
        {
            return new Ingredients
            {
                IngredientId = id
            };
        }

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
    }
}