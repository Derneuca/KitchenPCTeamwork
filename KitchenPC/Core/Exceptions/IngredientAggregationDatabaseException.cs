namespace KitchenPC.Exceptions
{
    using System;

    public class IngredientAggregationDatabaseException : Exception
    {
        public IngredientAggregationDatabaseException()
        {
        }

        public IngredientAggregationDatabaseException(string message)
            : base(message)
        {
        }
    }
}
