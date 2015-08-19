namespace KitchenPC.DB.Models
{
    using System;

    public class IngredientMetadata
    {
        public virtual Guid IngredientMetadataId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual bool? HasMeat { get; set; }

        public virtual float? CarbsPerUnit { get; set; }

        public virtual bool? HasRedMeat { get; set; }

        public virtual float? SugarPerUnit { get; set; }

        public virtual bool? HasPork { get; set; }

        public virtual float? FatPerUnit { get; set; }

        public virtual float? SodiumPerUnit { get; set; }

        public virtual float? CaloriesPerUnit { get; set; }

        public virtual short Spicy { get; set; }

        public virtual short Sweet { get; set; }

        public virtual bool? HasGluten { get; set; }

        public virtual bool? HasAnimal { get; set; }

        public virtual KitchenPC.Ingredients.IngredientMetadata AsIngredientMetadata()
        {
            return new KitchenPC.Ingredients.IngredientMetadata
            {
                HasMeat = this.HasMeat,
                CarbsPerUnit = this.CarbsPerUnit,
                HasRedMeat = this.HasRedMeat,
                SugarPerUnit = this.SugarPerUnit,
                HasPork = this.HasPork,
                FatPerUnit = this.FatPerUnit,
                SodiumPerUnit = this.SodiumPerUnit,
                CaloriesPerUnit = this.CaloriesPerUnit,
                Spicy = this.Spicy,
                Sweet = this.Sweet,
                HasGluten = this.HasGluten,
                HasAnimal = this.HasAnimal
            };
        }
    }
}