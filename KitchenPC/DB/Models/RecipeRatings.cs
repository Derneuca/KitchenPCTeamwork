namespace KitchenPC.DB.Models
{
    using System;

    public class RecipeRatings
    {
        public virtual Guid RatingId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual short Rating { get; set; }
    }
}