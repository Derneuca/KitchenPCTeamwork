namespace KitchenPC.Recipes
{
    using System;

    using KitchenPC.Recipes.Enums;
    using KitchenPC.Recipes.Filters;

    public class RecipeQuery
    {



        public string Keywords;
        public MealFilter Meal;
        public Rating? Rating;
        public Guid[] Include;
        public Guid[] Exclude;
        public Int32 Offset; //Used for paging
        public TimeFilter Time;
        public DietFilter Diet;
        public NutritionFilter Nutrition;
        public SkillFilter Skill;
        public TasteFilter Taste;
        public PhotoFilter Photos;
        public SortOrder Sort;
        public SortDirection Direction; //True if sort order is descending

        public RecipeQuery()
        {
            Taste.MildToSpicy = SpicinessLevel.Medium;
            Taste.SavoryToSweet = SweetnessLevel.Medium;

            Sort = SortOrder.Rating;
            Direction = SortDirection.Descending;
        }

        public RecipeQuery(RecipeQuery query)
        {
            this.Keywords = query.Keywords;
            this.Rating = query.Rating;
            if (query.Include != null) this.Include = (Guid[])query.Include.Clone();
            if (query.Exclude != null) this.Exclude = (Guid[])query.Exclude.Clone();
            this.Time = query.Time;
            this.Photos = query.Photos;
            this.Sort = query.Sort;
            this.Direction = query.Direction;
        }
    }
}