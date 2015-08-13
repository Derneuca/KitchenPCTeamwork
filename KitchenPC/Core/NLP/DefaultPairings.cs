namespace KitchenPC.NLP
{
    using Ingredients;

    public struct DefaultPairings
    {
        public static readonly DefaultPairings Empty = new DefaultPairings();

        public IngredientForm Unit { get; set; }

        public IngredientForm Volume { get; set; }

        public IngredientForm Weight { get; set; }

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