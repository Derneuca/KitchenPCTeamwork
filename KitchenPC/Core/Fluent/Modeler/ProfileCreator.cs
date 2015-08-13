namespace KitchenPC.Fluent.Modeler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KitchenPC.Ingredients;
    using KitchenPC.Modeler;
    using KitchenPC.Recipes;

    using IngredientUsage = KitchenPC.Ingredients.IngredientUsage;

    public class ProfileCreator
    {
        private readonly IList<Guid> blacklistIngredients;
        private readonly IList<RecipeRating> ratings;
        private readonly IList<PantryItem> pantry;
        private readonly IList<Guid> favoriteIngredients;
        private RecipeTags favoriteTags;
        private RecipeTags allowedTags;
        private Guid avoidRecipe;
        private Guid userid;

        public ProfileCreator()
        {
            this.ratings = new List<RecipeRating>();
            this.pantry = new List<PantryItem>();
            this.favoriteIngredients = new List<Guid>();
            this.blacklistIngredients = new List<Guid>();
        }

        public IUserProfile Profile
        {
            get
            {
                return new UserProfile
                {
                    UserId = this.userid,
                    Ratings = this.ratings.ToArray(),
                    Pantry = this.pantry.Any() ? this.pantry.ToArray() : null, // Pantry must be null or more than 0 items
                    FavoriteIngredients = this.favoriteIngredients.ToArray(),
                    FavoriteTags = this.favoriteTags,
                    BlacklistedIngredients = this.blacklistIngredients.ToArray(),
                    AvoidRecipe = this.avoidRecipe,
                    AllowedTags = this.allowedTags
                };
            }
        }

        public ProfileCreator WithUserId(Guid userid)
        {
            this.userid = userid;
            return this;
        }

        public ProfileCreator AddRating(RecipeRating rating)
        {
            this.ratings.Add(rating);
            return this;
        }

        public ProfileCreator AddRating(Recipe recipe, byte rating)
        {
            ratings.Add(new RecipeRating
            {
                RecipeId = recipe.Id,
                Rating = rating
            });

            return this;
        }

        public ProfileCreator AddPantryItem(PantryItem item)
        {
            this.pantry.Add(item);
            return this;
        }

        public ProfileCreator AddPantryItem(IngredientUsage usage)
        {
            this.pantry.Add(new PantryItem(usage));
            return this;
        }

        public ProfileCreator AddFavoriteIngredient(Ingredient ingredient)
        {
            this.favoriteIngredients.Add(ingredient.Id);
            return this;
        }

        public ProfileCreator FavoriteTags(RecipeTags tags)
        {
            this.favoriteTags = tags;
            return this;
        }

        public ProfileCreator AddBlacklistedIngredient(Ingredient ingredient)
        {
            this.blacklistIngredients.Add(ingredient.Id);
            return this;
        }

        public ProfileCreator AvoidRecipe(Recipe recipe)
        {
            this.avoidRecipe = recipe.Id;
            return this;
        }

        public ProfileCreator AllowedTags(RecipeTags tags)
        {
            this.allowedTags = tags;
            return this;
        }
    }
}
