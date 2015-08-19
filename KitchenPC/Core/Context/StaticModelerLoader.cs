namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Data;
    using KitchenPC.Data.DTO;
    using KitchenPC.Modeler;
    using KitchenPC.ShoppingLists;

    public class StaticModelerLoader : IModelerLoader
    {
        private readonly DataStore store;
        private IEnumerable<RecipeBinding> recipeData;
        private IEnumerable<IngredientBinding> ingredientData;
        private IEnumerable<RatingBinding> ratingData;

        public StaticModelerLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<RecipeBinding> LoadRecipeGraph()
        {
            if (this.recipeData != null)
            {
                return this.recipeData;
            }

            // Initialize Recipe Graph
            var metadata = this.store.GetIndexedRecipeMetadata();
            var graph = from recipe in this.store.Recipes
                        join m in metadata on recipe.RecipeId equals m.Key
                        select new RecipeBinding
                        {
                            Id = recipe.RecipeId,
                            Rating = Convert.ToByte(recipe.Rating),
                            Tags = RecipeMetadata.ToRecipeTags(m.Value)
                        };

            this.recipeData = graph;
            return this.recipeData;
        }

        public IEnumerable<IngredientBinding> LoadIngredientGraph()
        {
            if (this.ingredientData != null)
            {
                return this.ingredientData;
            }

            var forms = this.store.GetIndexedIngredientForms();
            var ingredients = this.store.GetIndexedIngredients();
            var graph = from recipeIngredient in this.store.RecipeIngredients
                        join form in forms on recipeIngredient.IngredientFormId equals form.Key
                        join ingredient in ingredients on recipeIngredient.IngredientId equals ingredient.Key
                        where ingredient.Key != ShoppingList.GuidWater
                        select IngredientBinding.Create(
                           ingredient.Key,
                           recipeIngredient.RecipeId,
                           recipeIngredient.Qty,
                           recipeIngredient.Unit,
                           ingredient.Value.ConversionType,
                           ingredient.Value.UnitWeight,
                           form.Value.UnitType,
                           form.Value.FormAmount,
                           form.Value.FormUnit);

            this.ingredientData = graph;
            return this.ingredientData;
        }

        public IEnumerable<RatingBinding> LoadRatingGraph()
        {
            if (this.ratingData != null)
            {
                return this.ratingData;
            }

            var graph = from r in this.store.RecipeRatings
                        select new RatingBinding
                        {
                            RecipeId = r.RecipeId,
                            UserId = r.UserId,
                            Rating = r.Rating
                        };

            this.ratingData = graph;
            return this.ratingData;
        }
    }
}