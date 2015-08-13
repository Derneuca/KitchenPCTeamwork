namespace KitchenPC.Data.DTO
{
    using System;

    public class RecipeRatings
    {
        public Guid RatingId { get; set; }

        public Guid UserId { get; set; }

        public Guid RecipeId { get; set; }

        public short Rating { get; set; }
    }
}