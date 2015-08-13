namespace KitchenPC.NLP
{
    using Enums;
    using Ingredients;

    public class Match : Result
    {
        protected IngredientUsage usage;

        public Match(string input, IngredientUsage usage)
           : base(input)
        {
            this.usage = usage;
        }

        public override IngredientUsage Usage
        {
            get
            {
                return this.usage;
            }
        }

        public override MatchResult Status
        {
            get
            {
                return MatchResult.Match;
            }
        }

        public override string ToString()
        {
            return string.Format("[Match] Usage: {0}", this.usage);
        }
    }
}