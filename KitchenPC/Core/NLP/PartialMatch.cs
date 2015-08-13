namespace KitchenPC.NLP
{
    using Enums;
    using Ingredients;

    public class PartialMatch : Match
    {
        public PartialMatch(string input, Ingredient ingredient, string prepNote)
            : base(input, null)
        {
            this.usage = new IngredientUsage(ingredient, null, null, prepNote);
        }

        public override MatchResult Status
        {
            get
            {
                return MatchResult.PartialMatch;
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.usage.PrepNote))
            {
                return string.Format("[PartialMatch] Ingredient: {0}", this.usage.Ingredient.Name);
            }
            else
            {
                return string.Format("[PartialMatch] Ingredient: {0} ({1})", this.usage.Ingredient.Name, this.usage.PrepNote);
            }
        }
    }
}