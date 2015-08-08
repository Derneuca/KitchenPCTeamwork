namespace KitchenPC.Categorization
{
    internal class Ranking
    {
        public Ranking(Category type)
        {
            this.Type = type;
            this.Score = 0f;
        }

        public float Score { get; set; }

        public Category Type { get; private set; }
    }
}