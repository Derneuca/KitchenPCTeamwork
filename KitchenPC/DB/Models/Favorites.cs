namespace KitchenPC.DB.Models
{
    using System;

    public class Favorites
    {
        public virtual Guid FavoriteId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual Menus Menu { get; set; }
    }
}