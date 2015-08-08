namespace KitchenPC.Categorization
{
    using KitchenPC.Categorization.Interfaces;
    using KitchenPC.Ingredients;

    internal class IngredientToken : IToken
    {
        private readonly Ingredient ingredient;

        public IngredientToken(Ingredient ingredient)
        {
            this.ingredient = ingredient;
        }

        public override bool Equals(object obj)
        {
            var token = obj as IngredientToken;
            bool result = token != null && token.ingredient.Id.Equals(this.ingredient.Id);
            return result;
        }

        public override int GetHashCode()
        {
            return this.ingredient.Id.GetHashCode();
        }

        public override string ToString()
        {
            return "[ING] - " + this.ingredient;
        }
    }
}