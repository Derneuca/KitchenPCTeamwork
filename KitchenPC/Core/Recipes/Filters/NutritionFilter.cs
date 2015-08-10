namespace KitchenPC.Recipes.Filters
{
    public class NutritionFilter
    {
        public bool LowCalorie { get; set; }

        public bool LowCarb { get; set; }

        public bool LowFat { get; set; }

        public bool LowSodium { get; set; }

        public bool LowSugar { get; set; }

        public static implicit operator bool(NutritionFilter filter)
        {
            bool result =
                filter.LowCalorie || 
                filter.LowCarb || 
                filter.LowFat || 
                filter.LowSodium || 
                filter.LowSugar;
            return result;
        }
    }
}
