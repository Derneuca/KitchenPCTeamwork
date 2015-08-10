namespace KitchenPC.Recipes.Filters
{
    public struct DietFilter
    {
        public bool GlutenFree { get; set; }

        public bool NoAnimals { get; set; }

        public bool NoMeat { get; set; }

        public bool NoPork { get; set; }

        public bool NoRedMeat { get; set; }

        public static implicit operator bool(DietFilter filter)
        {
            bool result =  
                filter.GlutenFree || 
                filter.NoAnimals || 
                filter.NoMeat || 
                filter.NoPork || 
                filter.NoRedMeat;
            return result;
        }
    }
}
