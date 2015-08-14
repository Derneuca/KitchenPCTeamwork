namespace KitchenPC.Modeler
{
    using System;
    using KitchenPC.Recipes;

    /// <summary>
    /// Interface used to transfer user specific runtime properties into the modeler.  KPC will be implementation that builds
    /// this information from the database, and unit tests will create static instances for testing.
    /// </summary>
    public interface IUserProfile
    {
        Guid? AvoidRecipe { get; }

        // DB User ID
        Guid UserId { get; }

        // Ingredients to always avoid
        Guid[] BlacklistedIngredients { get; }

        // Ingredients to favor
        Guid[] FavoriteIngredients { get; }

        // Set of ingredients user has available with amounts
        PantryItem[] Pantry { get; }

        // Every recipe ID user has rated with the rating
        RecipeRating[] Ratings { get; }

        // Tags to favor
        RecipeTags FavoriteTags { get; }

        // Only allow recipes that contain at least one of these tags (null to allow all)
        RecipeTags AllowedTags { get; } 
    }
}