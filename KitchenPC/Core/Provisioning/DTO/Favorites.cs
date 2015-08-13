namespace KitchenPC.Data.DTO
{
    using System;

    public class Favorites
    {
        public Guid FavoriteId { get; set; }

        public Guid UserId { get; set; }

        public Guid RecipeId { get; set; }

        public Guid? MenuId { get; set; }
    }
}