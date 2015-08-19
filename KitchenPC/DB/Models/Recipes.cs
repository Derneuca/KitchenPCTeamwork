namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;

    using KitchenPC.Recipes;

    public class Recipes
    {
        public virtual Guid RecipeId { get; set; }

        public virtual short? CookTime { get; set; }

        public virtual string Steps { get; set; }

        public virtual short? PrepTime { get; set; }

        public virtual short Rating { get; set; }

        public virtual string Description { get; set; }

        public virtual string Title { get; set; }

        public virtual bool Hidden { get; set; }

        public virtual string Credit { get; set; }

        public virtual string CreditUrl { get; set; }

        public virtual DateTime DateEntered { get; set; }

        public virtual short ServingSize { get; set; }

        public virtual string ImageUrl { get; set; }

        public virtual IList<RecipeIngredients> Ingredients { get; set; }

        public virtual RecipeMetadata RecipeMetadata { get; set; }

        public static Recipes FromId(Guid id)
        {
            return new Recipes
            {
                RecipeId = id
            };
        }

        public virtual RecipeBrief AsRecipeBrief()
        {
            return new RecipeBrief
            {
                Id = this.RecipeId,
                ImageUrl = this.ImageUrl,
                AvgRating = this.Rating,
                CookTime = this.CookTime,
                PrepTime = this.PrepTime,
                Description = this.Description,
                Title = this.Title
            };
        }
    }
}