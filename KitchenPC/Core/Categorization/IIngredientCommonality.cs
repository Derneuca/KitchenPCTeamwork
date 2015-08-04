namespace KitchenPC.Categorization
{
    using System;
    public interface IIngredientCommonality
    {
        Guid IngredientId { get; }
        Single Commonality { get; }
    }
}