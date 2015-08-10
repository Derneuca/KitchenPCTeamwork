namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;
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

        public static RecipeMetadata FromId(Guid id)
        {
            return new RecipeMetadata
            {
                RecipeMetadataId = id
            };
        }

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
    }

    public class RecipeMetadataMap : ClassMap<RecipeMetadata>
    {
        public RecipeMetadataMap()
        {
            this.Id(x => x.RecipeMetadataId)
               .GeneratedBy.GuidComb()
               .UnsavedValue(Guid.Empty);

            this.Map(x => x.Commonality).Not.Nullable().Index("IDX_RecipeMetadata_Commonality");
            this.Map(x => x.DietGlutenFree).Not.Nullable().Index("IDX_RecipeMetadata_DietGlutenFree");
            this.Map(x => x.DietNoAnimals).Not.Nullable().Index("IDX_RecipeMetadata_DietNoAnimals");
            this.Map(x => x.DietNomeat).Not.Nullable().Index("IDX_RecipeMetadata_DietNomeat");
            this.Map(x => x.DietNoPork).Not.Nullable().Index("IDX_RecipeMetadata_DietNoPork");
            this.Map(x => x.DietNoRedMeat).Not.Nullable().Index("IDX_RecipeMetadata_DietNoRedMeat");
            this.Map(x => x.MealBreakfast).Not.Nullable().Index("IDX_RecipeMetadata_MealBreakfast");
            this.Map(x => x.MealDessert).Not.Nullable().Index("IDX_RecipeMetadata_MealDessert");
            this.Map(x => x.MealDinner).Not.Nullable().Index("IDX_RecipeMetadata_MealDinner");
            this.Map(x => x.MealLunch).Not.Nullable().Index("IDX_RecipeMetadata_MealLunch");
            this.Map(x => x.NutritionLowCalorie).Not.Nullable().Index("IDX_RecipeMetadata_NutritionLowCalorie");
            this.Map(x => x.NutritionLowCarb).Not.Nullable().Index("IDX_RecipeMetadata_NutritionLowCarb");
            this.Map(x => x.NutritionLowFat).Not.Nullable().Index("IDX_RecipeMetadata_NutritionLowFat");
            this.Map(x => x.NutritionLowSodium).Not.Nullable().Index("IDX_RecipeMetadata_NutritionLowSodium");
            this.Map(x => x.NutritionLowSugar).Not.Nullable().Index("IDX_RecipeMetadata_NutritionLowSugar");
            this.Map(x => x.NutritionTotalCalories).Not.Nullable().Index("IDX_RecipeMetadata_NutritionTotalCalories");
            this.Map(x => x.NutritionTotalCarbs).Not.Nullable().Index("IDX_RecipeMetadata_NutritionTotalCarbs");
            this.Map(x => x.NutritionTotalfat).Not.Nullable().Index("IDX_RecipeMetadata_NutritionTotalfat");
            this.Map(x => x.NutritionTotalSodium).Not.Nullable().Index("IDX_RecipeMetadata_NutritionTotalSodium");
            this.Map(x => x.NutritionTotalSugar).Not.Nullable().Index("IDX_RecipeMetadata_NutritionTotalSugar");
            this.Map(x => x.PhotoRes).Not.Nullable().Index("IDX_RecipeMetadata_PhotoRes");
            this.Map(x => x.SkillCommon).Not.Nullable().Index("IDX_RecipeMetadata_SkillCommon");
            this.Map(x => x.SkillEasy).Not.Nullable().Index("IDX_RecipeMetadata_SkillEasy");
            this.Map(x => x.SkillQuick).Not.Nullable().Index("IDX_RecipeMetadata_SkillQuick");
            this.Map(x => x.TasteMildToSpicy).Not.Nullable().Index("IDX_RecipeMetadata_TasteMildToSpicy");
            this.Map(x => x.TasteSavoryToSweet).Not.Nullable().Index("IDX_RecipeMetadata_TasteSavoryToSweet");
            this.Map(x => x.UsdaMatch).Not.Nullable();

            this.References(x => x.Recipe).Not.Nullable().Unique().Index("IDX_RecipeMetadata_RecipeId");
        }
    }
}