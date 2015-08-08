namespace KitchenPC.Categorization
{
    public class CategorizationResult
    {
        public bool DietGlutenFree;
        public bool DietNoAnimals;
        public bool DietNoMeat;
        public bool DietNoPork;
        public bool DietNoRedMeat;
        public bool MealBreakfast;
        public bool MealDessert;
        public bool MealDinner;
        public bool MealLunch;
        public bool NutritionLowCalorie;
        public bool NutritionLowCarb;
        public bool NutritionLowFat;
        public bool Nutrition_LowSodium;
        public bool Nutrition_LowSugar;
        public short Nutrition_TotalCalories;
        public short Nutrition_TotalCarbs;
        public short Nutrition_TotalFat;
        public short Nutrition_TotalSodium;
        public short Nutrition_TotalSugar;
        public bool Skill_Easy;
        public bool Skill_Quick;
        public bool Skill_Common;
        public byte Taste_MildToSpicy;
        public byte Taste_SavoryToSweet;
        public float Commonality;

        public bool USDAMatch;
    }
}