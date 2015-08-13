namespace KitchenPC.NLP
{
    using Ingredients;
    using Enums;

    public class AnomalousMatch : Match
    {
        private readonly AnomalousResult anomaly;

        public AnomalousMatch(string input, AnomalousResult anomaly, IngredientUsage usage)
            : base(input, usage)
        {
            this.anomaly = anomaly;
        }

        public override string ToString()
        {
            return string.Format("[AnomalousMatch] ({0}) Usage: {1}", anomaly, usage);
        }
    }
}