namespace KitchenPC.Recipes
{
    using System;

    public class RecipeTag
    {
        public const int NumberOfTags = 17;

        public static readonly RecipeTag GlutenFree = new RecipeTag(0, GlutenFreeLabel);
        public static readonly RecipeTag NoAnimals = new RecipeTag(1, NoAnimalsLabel);
        public static readonly RecipeTag NoMeat = new RecipeTag(2, NoMeatLabel);
        public static readonly RecipeTag NoPork = new RecipeTag(3, NoPorkLabel);
        public static readonly RecipeTag NoRedMeat = new RecipeTag(4, NoRedMeatLabel);
        public static readonly RecipeTag Breakfast = new RecipeTag(5, BreakfastLabel);
        public static readonly RecipeTag Dessert = new RecipeTag(6, DessertLabel);
        public static readonly RecipeTag Dinner = new RecipeTag(7, DinnerLabel);
        public static readonly RecipeTag Lunch = new RecipeTag(8, LunchLabel);
        public static readonly RecipeTag LowCalorie = new RecipeTag(9, LowCalorieLabel);
        public static readonly RecipeTag LowCarb = new RecipeTag(10, LowCarbLabel);
        public static readonly RecipeTag LowFat = new RecipeTag(11, LowFatLabel);
        public static readonly RecipeTag LowSodium = new RecipeTag(12, LowSodiumLabel);
        public static readonly RecipeTag LowSugar = new RecipeTag(13, LowSugarLabel);
        public static readonly RecipeTag CommonIngredients = new RecipeTag(14, CommonIngredientsLabel);
        public static readonly RecipeTag EasyToMake = new RecipeTag(15, EasyToMakeLabel);
        public static readonly RecipeTag Quick = new RecipeTag(16, QuickLabel);

        private const string GlutenFreeLabel = "Gluten Free";
        private const string NoAnimalsLabel = "No Animals";
        private const string NoMeatLabel = "No Meat";
        private const string NoPorkLabel = "No Pork";
        private const string NoRedMeatLabel = "No Red Meat";
        private const string BreakfastLabel = "Breakfast";
        private const string DessertLabel = "Dessert";
        private const string DinnerLabel = "Dinner";
        private const string LunchLabel = "Lunch";
        private const string LowCalorieLabel = "Low Calorie";
        private const string LowCarbLabel = "Low Carb";
        private const string LowFatLabel = "Low Fat";
        private const string LowSodiumLabel = "Low Sodium";
        private const string LowSugarLabel = "Low Sugar";
        private const string CommonIngredientsLabel = "Common Ingredients";
        private const string EasyToMakeLabel = "Easy To Make";
        private const string QuickLabel = "Quick";

        private readonly int tagValue; // Ordinal value of tag
        private readonly int bitflag; // Bitmask value (power of 2)
        private readonly string label; // Name of tag

        public RecipeTag()
        {
        }

        private RecipeTag(int value, string label)
        {
            this.tagValue = value;
            this.bitflag = 1 << value;
            this.label = label;
        }

        public int Value
        {
            get
            {
                return this.tagValue;
            }
        }

        public int BitFlag
        {
            get
            {
                return this.bitflag;
            }
        }

        public static bool operator !=(RecipeTag firstTag, RecipeTag secondTag)
        {
            bool result = !(firstTag == secondTag);
            return result;
        }

        public static bool operator ==(RecipeTag firstTag, RecipeTag secondTag)
        {
            if (ReferenceEquals(firstTag, secondTag))
            {
                return true;
            }

            if ((object)firstTag == null || ((object)secondTag == null))
            {
                return false;
            }

            bool result = firstTag.tagValue == secondTag.tagValue;
            return result;
        }

        public static RecipeTags operator |(RecipeTag firstTag, RecipeTag secondTag)
        {
            var result = (RecipeTags)firstTag.bitflag | secondTag.bitflag;
            return result;
        }

        public static implicit operator string(RecipeTag tags)
        {
            string result = tags.label;
            return result;
        }

        public static implicit operator RecipeTag(string tag)
        {
            switch (tag)
            {
                case GlutenFreeLabel:
                    return GlutenFree;
                case NoAnimalsLabel:
                    return NoAnimals;
                case NoMeatLabel:
                    return NoMeat;
                case NoPorkLabel:
                    return NoPork;
                case NoRedMeatLabel:
                    return NoRedMeat;
                case BreakfastLabel:
                    return Breakfast;
                case DessertLabel:
                    return Dessert;
                case DinnerLabel: 
                    return Dinner;
                case LunchLabel:
                    return Lunch;
                case LowCalorieLabel:
                    return LowCalorie;
                case LowCarbLabel:
                    return LowCarb;
                case LowFatLabel:
                    return LowFat;
                case LowSodiumLabel:
                    return LowSodium;
                case LowSugarLabel:
                    return LowSugar;
                case CommonIngredientsLabel:
                    return CommonIngredients;
                case EasyToMakeLabel:
                    return EasyToMake;
                case QuickLabel:
                    return Quick;
                default:
                    throw new ArgumentException("Cannot parse recipe tag: " + tag);
            }
        }

        public override int GetHashCode()
        {
            int result = this.tagValue;
            return result;
        }

        public override string ToString()
        {
            string result = this.label;
            return result;
        }

        public override bool Equals(object obj)
        {
            var tag = obj as RecipeTag;
            bool result = tag != null && tag.tagValue == this.tagValue;
            return result;
        }        
    }
}