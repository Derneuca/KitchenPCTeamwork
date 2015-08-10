namespace KitchenPC.Recipes
{
    using System;

    using KitchenPC.Recipes.Enums;
    using KitchenPC.Recipes.Filters;

    public class RecipeQuery
    {
        public RecipeQuery()
        {
            this.Taste.MildToSpicy = SpicinessLevel.Medium;
            this.Taste.SavoryToSweet = SweetnessLevel.Medium;
            this.Sort = SortOrder.Rating;
            this.Direction = SortDirection.Descending;
        }

        public RecipeQuery(RecipeQuery query)
        {
            this.Keywords = query.Keywords;
            this.Rating = query.Rating;
            if (query.Include != null)
            {
                this.Include = (Guid[])query.Include.Clone();
            }

            if (query.Exclude != null)
            {
                this.Exclude = (Guid[])query.Exclude.Clone();
            }

            this.Time = query.Time;
            this.Photos = query.Photos;
            this.Sort = query.Sort;
            this.Direction = query.Direction;
        }

        public string Keywords { get; set; }

        // Used for paging
        public int Offset { get; set; }

        public Guid[] Include { get; set; }

        public Guid[] Exclude { get; set; }

        public TimeFilter Time { get; set; }

        public DietFilter Diet { get; set; }

        public NutritionFilter Nutrition { get; set; }

        public SkillFilter Skill { get; set; }

        public TasteFilter Taste { get; set; }

        public MealFilter Meal { get; set; }

        public PhotoFilter Photos { get; set; }

        public SortOrder Sort { get; set; }

        // True if sort order is descending
        public SortDirection Direction { get; set; }

        public Rating? Rating { get; set; }
    }
}