namespace KitchenPC.DB.Models
{
    using System;

    using KitchenPC.Recipes;

    public class RecipeMetadata
    {
        public virtual Guid RecipeMetadataId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual int PhotoRes { get; set; }

        public virtual float Commonality { get; set; }

        public virtual bool UsdaMatch { get; set; }

        public virtual bool MealBreakfast { get; set; }

        public virtual bool MealLunch { get; set; }

        public virtual bool MealDinner { get; set; }

        public virtual bool MealDessert { get; set; }

        public virtual bool DietNomeat { get; set; }

        public virtual bool DietGlutenFree { get; set; }

        public virtual bool DietNoRedMeat { get; set; }

        public virtual bool DietNoAnimals { get; set; }

        public virtual bool DietNoPork { get; set; }

        public virtual short NutritionTotalfat { get; set; }

        public virtual short NutritionTotalSodium { get; set; }

        public virtual bool NutritionLowSodium { get; set; }

        public virtual bool NutritionLowSugar { get; set; }

        public virtual bool NutritionLowCalorie { get; set; }

        public virtual short NutritionTotalSugar { get; set; }

        public virtual short NutritionTotalCalories { get; set; }

        public virtual bool NutritionLowFat { get; set; }

        public virtual bool NutritionLowCarb { get; set; }

        public virtual short NutritionTotalCarbs { get; set; }

        public virtual bool SkillQuick { get; set; }

        public virtual bool SkillEasy { get; set; }

        public virtual bool SkillCommon { get; set; }

        public virtual short TasteMildToSpicy { get; set; }

        public virtual short TasteSavoryToSweet { get; set; }

        public virtual RecipeTags Tags
        {
            get
            {
                var t = RecipeTags.None;

                if (this.DietGlutenFree)
                {
                    t |= RecipeTag.GlutenFree;
                }

                if (this.DietNoAnimals)
                {
                    t |= RecipeTag.NoAnimals;
                }

                if (this.DietNomeat)
                {
                    t |= RecipeTag.NoMeat;
                }

                if (this.DietNoPork)
                {
                    t |= RecipeTag.NoPork;
                }

                if (this.DietNoRedMeat)
                {
                    t |= RecipeTag.NoRedMeat;
                }

                if (this.MealBreakfast)
                {
                    t |= RecipeTag.Breakfast;
                }

                if (this.MealDessert)
                {
                    t |= RecipeTag.Dessert;
                }

                if (this.MealDinner)
                {
                    t |= RecipeTag.Dinner;
                }

                if (this.MealLunch)
                {
                    t |= RecipeTag.Lunch;
                }

                if (this.NutritionLowCalorie)
                {
                    t |= RecipeTag.LowCalorie;
                }

                if (this.NutritionLowCarb)
                {
                    t |= RecipeTag.LowCarb;
                }

                if (this.NutritionLowFat)
                {
                    t |= RecipeTag.LowFat;
                }

                if (this.NutritionLowSodium)
                {
                    t |= RecipeTag.LowSodium;
                }

                if (this.NutritionLowSugar)
                {
                    t |= RecipeTag.LowSugar;
                }

                if (this.SkillCommon)
                {
                    t |= RecipeTag.CommonIngredients;
                }

                if (this.SkillEasy)
                {
                    t |= RecipeTag.EasyToMake;
                }

                if (this.SkillQuick)
                {
                    t |= RecipeTag.Quick;
                }

                return t;
            }
        }

        public static RecipeMetadata FromId(Guid id)
        {
            return new RecipeMetadata
            {
                RecipeMetadataId = id
            };
        }     
    }
}