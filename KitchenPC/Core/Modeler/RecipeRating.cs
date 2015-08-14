namespace KitchenPC.Modeler
{
    using System;

    public struct RecipeRating
    {
        public Guid RecipeId { get; set; }
        public byte Rating { get; set; }
    }
}