namespace KitchenPC.Data.DTO
{
    using System;
    using KitchenPC.Recipes;

    public class Recipes
    {
        public DateTime DateEntered { get; set; }

        public Guid RecipeId { get; set; }

        public short? CookTime { get; set; }
        
        public short? PreparationTime { get; set; }

        public short Rating { get; set; }

        public short ServingSize { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string Steps { get; set; }

        public string Credit { get; set; }

        public string CreditUrl { get; set; }

        public string ImageUrl { get; set; }

        public bool Hidden { get; set; }
        
        public static RecipeBrief ToRecipeBrief(Recipes dtoRecipe)
        {
            return new RecipeBrief
            {
                Id = dtoRecipe.RecipeId,
                ImageUrl = dtoRecipe.ImageUrl,
                AvgRating = dtoRecipe.Rating,
                CookTime = dtoRecipe.CookTime,
                PrepTime = dtoRecipe.PreparationTime,
                Description = dtoRecipe.Description,
                Title = dtoRecipe.Title
            };
        }
    }
}