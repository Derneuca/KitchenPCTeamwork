namespace KitchenPC.Exceptions
{
    using KitchenPC.Ingredients;

    public class InvalidFormException : KPCException
    {
        public InvalidFormException(Ingredient ingredient, IngredientForm form)
        {
            this.Ingredient = ingredient;
            this.Form = form;
        }

        public Ingredient Ingredient { get; private set; }

        public IngredientForm Form { get; private set; }
    }
}
