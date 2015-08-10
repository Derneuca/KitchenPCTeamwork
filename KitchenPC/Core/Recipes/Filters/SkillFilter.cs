namespace KitchenPC.Recipes.Filters
{
    public class SkillFilter
    {
        public bool Common { get; set; }

        public bool Easy { get; set; }

        public bool Quick { get; set; }

        public static implicit operator bool(SkillFilter filter)
        {
            bool result = filter.Common || filter.Easy || filter.Quick;
            return result;
        }
    }
}
