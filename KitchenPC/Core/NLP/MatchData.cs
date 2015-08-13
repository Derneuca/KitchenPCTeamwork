namespace KitchenPC.NLP
{
    public class MatchData
    {
        public MatchData()
        {
            this.Preps = new Preps();
        }

        public IngredientNode Ingredient { get; set; }

        public UnitNode Unit { get; set; }

        public FormNode Form { get; set; }

        public Amount Amount { get; set; }

        public Preps Preps { get; set; }
    }
}