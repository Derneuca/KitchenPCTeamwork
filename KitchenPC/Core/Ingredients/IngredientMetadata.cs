namespace KitchenPC.Ingredients
{
    public class IngredientMetadata
    {
        public IngredientMetadata()
        {
        }

        public IngredientMetadata(
           bool? hasGluten,
           bool? hasMeat,
           bool? hasRedMeat,
           bool? hasPork,
           bool? hasAnimal,
           byte spicy,
           byte sweet,
           float? fatPerUnit,
           float? sugarPerUnit,
           float? caloriesPerUnit,
           float? sodiumPerUnit,
           float? carbsPerUnit)
        {
            this.HasGluten = hasGluten;
            this.HasMeat = hasMeat;
            this.HasRedMeat = hasRedMeat;
            this.HasPork = hasPork;
            this.HasAnimal = hasAnimal;
            this.Spicy = spicy;
            this.Sweet = sweet;
            this.FatPerUnit = fatPerUnit;
            this.SugarPerUnit = sugarPerUnit;
            this.CaloriesPerUnit = caloriesPerUnit;
            this.SodiumPerUnit = sodiumPerUnit;
            this.CarbsPerUnit = carbsPerUnit;
        }

        public bool? HasGluten { get; set; }

        public bool? HasMeat { get; set; }

        public bool? HasRedMeat { get; set; }

        public bool? HasPork { get; set; }

        public bool? HasAnimal { get; set; }

        public short Spicy { get; set; }

        public short Sweet { get; set; }

        public float? FatPerUnit { get; set; }

        public float? SugarPerUnit { get; set; }

        public float? CaloriesPerUnit { get; set; }

        public float? SodiumPerUnit { get; set; }

        public float? CarbsPerUnit { get; set; }
    }
}