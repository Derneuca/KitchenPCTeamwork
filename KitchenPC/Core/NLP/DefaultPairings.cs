namespace KitchenPC.NLP
{
    using Ingredients;

    public struct DefaultPairings
    {
        public static DefaultPairings Empty { get; } = new DefaultPairings();

        public IngredientForm Unit;
        public IngredientForm Volume;
        public IngredientForm Weight;

        public bool IsEmpty
        {
            get
            {
                return (this.Unit == null && this.Volume == null && this.Weight == null);
            }
        }

        public bool HasUnit
        {
            get
            {
                return this.Unit != null;
            }
        }

        public bool HasVolume
        {
            get
            {
                return this.Volume != null;
            }
        }

        public bool HasWeight
        {
            get
            {
                return this.Weight != null;
            }
        }
    }
}