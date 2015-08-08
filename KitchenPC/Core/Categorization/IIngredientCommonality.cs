namespace KitchenPC.Categorization
{
    using System;

    public interface IIngredientCommonality
    {
        Guid IngredientId { get; }

        float Commonality { get; }
    }
}