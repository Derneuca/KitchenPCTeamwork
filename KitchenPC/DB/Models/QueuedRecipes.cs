namespace KitchenPC.DB.Models
{
    using System;

    public class QueuedRecipes
    {
        public virtual Guid QueueId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual DateTime QueuedDate { get; set; }
    }
}