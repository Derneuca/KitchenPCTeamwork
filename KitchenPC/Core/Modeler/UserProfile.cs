namespace KitchenPC.Modeler
{
    using System;
    using KitchenPC.Recipes;
    
    public class UserProfile : IUserProfile
    {
        private static IUserProfile anonymous;

        /// <summary>
        /// Gets a modeling profile that has no user context, such as a saved pantry, favorite ingredients, blacklists, etc.
        /// </summary>
        public static IUserProfile Anonymous
        {
            get
            {
                if (anonymous == null)
                {
                    anonymous = new UserProfile
                    {
                        UserId = Guid.Empty,
                        Ratings = new RecipeRating[0],
                        FavoriteIngredients = new Guid[0],
                        FavoriteTags = RecipeTags.None,
                        BlacklistedIngredients = new Guid[0]
                    };
                }

                return anonymous;
            }
        }

        public Guid UserId { get; set; }

        public Guid? AvoidRecipe { get; set; }

        public Guid[] BlacklistedIngredients { get; set; }

        public Guid[] FavoriteIngredients { get; set; }

        public PantryItem[] Pantry { get; set; }

        public RecipeRating[] Ratings { get; set; }

        public RecipeTags FavoriteTags { get; set; }

        public RecipeTags AllowedTags { get; set; }
    }
}