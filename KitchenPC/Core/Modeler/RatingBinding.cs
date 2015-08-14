namespace KitchenPC.Modeler
{
    using System;

    public struct RatingBinding
    {
        public Guid UserId { get; set; }

        public Guid RecipeId { get; set; }

        public short Rating { get; set; }
    }
}