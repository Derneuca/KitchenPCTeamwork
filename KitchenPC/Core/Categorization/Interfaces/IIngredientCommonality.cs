namespace KitchenPC.Categorization.Interfaces
{
    using System;

    public interface IIngredientCommonality
    {
        Guid IngredientId { get; }

        float Commonality { get; }
    }
}