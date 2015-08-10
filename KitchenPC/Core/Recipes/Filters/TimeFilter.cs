namespace KitchenPC.Recipes.Filters
{
    public class TimeFilter
    {
        public short? MaxPrep { get; set; }

        public short? MaxCook { get; set; }

        public static implicit operator bool(TimeFilter filter)
        {
            bool result = filter.MaxPrep.HasValue || filter.MaxCook.HasValue;
            return result;
        }
    }
}
