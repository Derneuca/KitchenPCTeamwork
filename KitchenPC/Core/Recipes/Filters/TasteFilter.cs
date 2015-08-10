namespace KitchenPC.Recipes.Filters
{
    using KitchenPC.Recipes.Enums;

    public struct TasteFilter
    {
        private static readonly byte[] SpicyOffsets = { 0, 2, 0, 3, 10 };
        private static readonly byte[] SweetOffsets = { 3, 10, 0, 20, 30 };

        public SpicinessLevel MildToSpicy { get; set; }

        public SweetnessLevel SavoryToSweet { get; set; }

        public byte Spiciness
        {
            get
            {
                return SpicyOffsets[(int)this.MildToSpicy];
            }
        }

        public byte Sweetness
        {
            get
            {
                return SweetOffsets[(int)this.SavoryToSweet];
            }
        }

        public static implicit operator bool(TasteFilter filter)
        {
            bool result = 
                filter.MildToSpicy != SpicinessLevel.Medium || 
                filter.SavoryToSweet != SweetnessLevel.Medium;
            return result;
        }
    }
}
