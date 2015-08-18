namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using KitchenPC.Context;
    using KitchenPC.Recipes;
    using log4net;

    /// <summary>
    /// Represents a modeling session for a given user with a given pantry.
    /// This object can be re-used (or cached) while the user changes modeling preferences and remodels.
    /// </summary>
    public class ModelingSession
    {
        private const int MaxSuggestions = 15;
        private const double CoolingRate = 0.9999;
        private const float MissingIngredientPunish = 5.0f;
        private const float NewIngredientPunish = 2.0f;
        private const float EmptyRecipeAmount = 0.50f;

        private readonly Random random = new Random();
        private readonly IngredientNode[] pantryIngredients;
        private readonly Dictionary<IngredientNode, float?> pantryAmounts;
        private readonly List<IngredientNode> ingredientBlacklist;
        private readonly Dictionary<RecipeNode, byte> ratings;
        private readonly DBSnapshot dataBase;
        private readonly IKPCContext context;
        private readonly IUserProfile profile;

        // Truth table of favorite tags
        private readonly bool[] favoriteTags;

        // Array of top 5 favorite ingredients by id
        private readonly int[] favoriteIngredients;

        // Copy of profile
        private readonly RecipeTags allowedTags;

        // Hold totals for each scoring round so we don't have to reallocate map every time
        private Dictionary<IngredientNode, IngredientUsage> totals;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelingSession"/> class.
        /// </summary>
        /// <param name="context">KitchenPC context used for this modeling session.</param>
        /// <param name="db">Object containing all available recipes, ratings, and trend information.</param>
        /// <param name="profile">Object containing user specific information, such as pantry and user ratings.</param>
        public ModelingSession(IKPCContext context, DBSnapshot db, IUserProfile profile)
        {
            Log = LogManager.GetLogger(typeof(ModelingSession));
            this.dataBase = db;
            this.context = context;
            this.profile = profile;
            this.favoriteTags = new bool[RecipeTag.NumberOfTags];
            this.favoriteIngredients = new int[profile.FavoriteIngredients.Length];

            // Empty pantries must be null, not zero items
            if (this.profile.Pantry != null && this.profile.Pantry.Length == 0) 
            {
                throw new EmptyPantryException();
            }

            if (this.profile.AllowedTags != null)
            {
                this.allowedTags = this.profile.AllowedTags;
            }

            if (this.profile.Pantry != null)
            {
                this.pantryAmounts = new Dictionary<IngredientNode, float?>();
                foreach (var item in this.profile.Pantry)
                {
                    var node = this.dataBase.FindIngredient(item.IngredientId);

                    // If an ingredient isn't used by any recipe, there's no reason for it to be in the pantry.
                    if (node == null)
                    {
                        continue;
                    }

                    // If an ingredient exists, but doesn't have any link to any allowed tags, there's no reason for it to be in the pantry.
                    if (this.allowedTags != null && (node.AvailableTags & this.allowedTags) == 0)
                    {
                        continue;
                    }

                    if (this.pantryAmounts.ContainsKey(node))
                    {
                        throw new DuplicatePantryItemException();
                    }

                    this.pantryAmounts.Add(node, item.Amount);
                }

                if (this.pantryAmounts.Keys.Count == 0)
                {
                    throw new ImpossibleQueryException();
                }

                this.pantryIngredients = this.pantryAmounts.Keys.ToArray();
            }

            if (this.profile.FavoriteIngredients != null)
            {
                var i = 0;
                foreach (var ingredient in this.profile.FavoriteIngredients)
                {
                    var node = this.dataBase.FindIngredient(ingredient);
                    this.favoriteIngredients[i] = node.Key;
                }
            }

            if (this.profile.FavoriteTags != null)
            {
                foreach (var tag in this.profile.FavoriteTags)
                {
                    this.favoriteTags[tag.Value] = true;
                }
            }

            if (this.profile.BlacklistedIngredients != null)
            {
                this.ingredientBlacklist = new List<IngredientNode>();
                foreach (var ingredient in this.profile.BlacklistedIngredients)
                {
                    var node = this.dataBase.FindIngredient(ingredient);
                    this.ingredientBlacklist.Add(node);
                }
            }

            if (this.profile.Ratings != null)
            {
                this.ratings = new Dictionary<RecipeNode, byte>(profile.Ratings.Length);
                foreach (var recipe in this.profile.Ratings)
                {
                    var node = this.dataBase.FindRecipe(recipe.RecipeId);
                    this.ratings.Add(node, recipe.Rating);
                }
            }
            else
            {
                this.ratings = new Dictionary<RecipeNode, byte>(0);
            }
        }

        public static ILog Log { get; set; }

        /// <summary>
        /// Generates a model with the specified number of recipes and returns the recipe IDs in the optimal order.
        /// </summary>
        /// <param name="recipes">Number of recipes to generate</param>
        /// <param name="scale">Scale indicating importance of optimal ingredient usage vs. user trend usage. 1 indicates ignore user trends, return most efficient set of recipes. 5 indicates ignore pantry and generate recipes user is most likely to rate high.</param>
        /// <returns>An array up to size "recipes" containing recipes from DBSnapshot.</returns>
        public Model Generate(int recipes, byte scale)
        {
            if (recipes > MaxSuggestions)
            {
                throw new ArgumentException("Modeler can only generate " + MaxSuggestions.ToString() + " recipes at a time.");
            }

            double temperature = 10000.0;
            double deltaScore = 0;
            const double AbsoluteTemperature = 0.00001;

            this.totals = new Dictionary<IngredientNode, IngredientUsage>(IngredientNode.NextKey);

            // Current set of recipes
            var currentSet = new RecipeNode[recipes];

            // Set to compare with current
            var nextSet = new RecipeNode[recipes];

            // Initialize with n random recipes
            this.InitializeSet(currentSet);

            // Check initial score
            var score = this.GetScore(currentSet, scale); 

            var timer = new Stopwatch();
            timer.Start();

            while (temperature > AbsoluteTemperature)
            {
                nextSet = this.GetNextSet(currentSet); // Swap out a random recipe with another one from the available pool
                deltaScore = this.GetScore(nextSet, scale) - score;

                // if the new set has a smaller score (good thing)
                // or if the new set has a higher score but satisfies Boltzman condition then accept the set
                if ((deltaScore < 0) || (score > 0 && Math.Exp(-deltaScore / temperature) > this.random.NextDouble()))
                {
                    nextSet.CopyTo(currentSet, 0);
                    score += deltaScore;
                }

                // cool down the temperature
                temperature *= CoolingRate;
            }

            timer.Stop();
            Log.InfoFormat("Generating set of {0} recipes took {1}ms.", recipes, timer.ElapsedMilliseconds);

            var model = new Model(currentSet, this.profile.Pantry, score);
            return model;
        }

        /// <summary>Takes a model generated from the modeling engine and loads necessary data from the database to deliver relevance to a user interface.</summary>
        /// <param name="model">Model from modeling engine</param>
        /// <returns>CompiledModel object which contains full recipe information about the provided set.</returns>
        public CompiledModel Compile(Model model)
        {
            var results = new CompiledModel();
            var recipes = this.context.ReadRecipes(model.RecipeIds, ReadRecipeOptions.None);

            results.RecipeIds = model.RecipeIds;
            results.Pantry = model.Pantry;
            results.Briefs = recipes.Select(r => { return new RecipeBrief(r); }).ToArray();
            results.Recipes = recipes.Select(r => new SuggestedRecipe
            {
                Id = r.Id,
                Ingredients = this.context.AggregateRecipes(r.Id).ToArray()
            }).ToArray();

            return results;
        }

        /// <summary>
        /// Judges a set of recipes based on a scale and its efficiency with regards to the current pantry.  The lower the score, the better.
        /// </summary>
        private double GetScore(RecipeNode[] currentSet, byte scale)
        {
            double wasted = 0; // Add 1.0 for ingredients that don't exist in pantry, add percentage of leftover otherwise
            float averageRating = 0; // Average rating for all recipes in the set (0-4)
            float tagPoints = 0; // Point for each tag that's one of our favorites
            float tagTotal = 0; // Total number of tags in all recipes
            float ingredientPoints = 0; // Point for each ingredient that's one of our favorites
            float ingredientTotal = 0; // Total number of ingrediets in all recipes

            for (var i = 0; i < currentSet.Length; i++)
            {
                var recipe = currentSet[i];
                var ingredients = (IngredientUsage[])recipe.Ingredients;

                // Add points for any favorite tags this recipe uses
                tagTotal += recipe.Tags.Length;
                ingredientTotal += ingredients.Length;

                // TODO: Use bitmasks for storing recipe tags and fav tags, then count bits
                for (int tag = 0; tag < recipe.Tags.Length; tag++) 
                {
                    if (this.favoriteTags[tag])
                    {
                        tagPoints++;
                    }
                }

                // Real rating is the user's rating, else the public rating, else 3.
                byte realRating; 
                if (!this.ratings.TryGetValue(recipe, out realRating))
                {
                    // if recipe has no ratings, use average rating of 3.
                    realRating = (recipe.Rating == 0) ? (byte)3 : recipe.Rating; 
                }

                averageRating += realRating - 1;

                for (var j = 0; j < ingredients.Length; j++)
                {
                    var ingredient = ingredients[j];

                    // Add points for any favorite ingredients this recipe uses
                    var ingredientKey = ingredient.Ingredient.Key;

                    for (var key = 0; key < this.favoriteIngredients.Length; key++) 
                    {
                        if (this.favoriteIngredients[key] == ingredientKey)
                        {
                            ingredientPoints++;
                            break;
                        }
                    }

                    IngredientUsage currentUsage;
                    bool fContains = this.totals.TryGetValue(ingredient.Ingredient, out currentUsage);
                    if (!fContains)
                    {
                        currentUsage = new IngredientUsage();
                        currentUsage.Amount = ingredient.Amount;
                        this.totals.Add(ingredient.Ingredient, currentUsage);
                    }
                    else
                    {
                        currentUsage.Amount += ingredient.Amount;
                    }
                }
            }

            // If profile has a pantry, figure out how much of it is wasted
            if (this.profile.Pantry != null) 
            {
                // For each pantry ingredient that we're not using, punish the score by MissingIngredientPunish amount.
                var pantryEnumerator = this.pantryAmounts.GetEnumerator();
                while (pantryEnumerator.MoveNext())
                {
                    if (!this.totals.ContainsKey(pantryEnumerator.Current.Key))
                    {
                        wasted += MissingIngredientPunish;
                    }
                }

                var enumerator = this.totals.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var currentKey = enumerator.Current.Key;

                    float? haveAmount;

                    // We have this in our pantry
                    if (this.pantryAmounts.TryGetValue(currentKey, out haveAmount)) 
                    {
                        // We have this in our pantry, but no amount is specified - So we "act" like we have whatever we need
                        if (!haveAmount.HasValue) 
                        {
                            continue;
                        }

                        // This recipe doesn't specify an amount - So we "act" like we use half of what we have
                        if (!enumerator.Current.Value.Amount.HasValue) 
                        {
                            wasted += EmptyRecipeAmount;
                            continue;
                        }

                        float needAmount = enumerator.Current.Value.Amount.Value;

                        // Percentage of how much you're using of what you have
                        float ratio = 1 - ((haveAmount.Value - needAmount) / haveAmount.Value); 

                        // If you need more than you have, add the excess ratio to the waste but don't go over the punishment for not having the ingredient at all
                        if (ratio > 1) 
                        {
                            wasted += Math.Min(ratio, NewIngredientPunish);
                        }
                        else
                        {
                            wasted += 1 - ratio;
                        }
                    }
                    else
                    {
                        // For each ingredient this meal set needs that we don't have, increment by NewIngredientPunish
                        wasted += NewIngredientPunish; 
                    }
                }
            }

            double worstScore;
            double trendScore;
            double efficiencyScore;

            // No pantry, efficiency is defined by the overlap of ingredients across recipes
            if (this.profile.Pantry == null) 
            {
                efficiencyScore = this.totals.Keys.Count / ingredientTotal;
            }
            else 
            {
                // Efficiency is defined by how efficient the pantry ingredients are utilized
                worstScore = 
                    this.totals.Keys.Count * NewIngredientPunish + 
                    this.profile.Pantry.Length * MissingIngredientPunish; // Worst possible efficiency score
                efficiencyScore = wasted / worstScore;
            }

            averageRating /= currentSet.Length;
            trendScore = 1 - ((averageRating + tagPoints / tagTotal + ingredientPoints / ingredientTotal) / 6);

            this.totals.Clear();

            switch (scale)
            {
                case 1:
                    return efficiencyScore;
                case 2:
                    return (efficiencyScore + efficiencyScore + trendScore) / 3;
                case 3:
                    return (efficiencyScore + trendScore) / 2;
                case 4:
                    return (efficiencyScore + trendScore + trendScore) / 3;
                case 5:
                    return trendScore;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Initializes currentSet with random recipes from the available recipe pool.
        /// </summary>
        private void InitializeSet(RecipeNode[] currentSet)
        {
            var inUse = new List<Guid>(currentSet.Length);

            for (var i = 0; i < currentSet.Length; i++)
            {
                RecipeNode node;
                var timeout = 0;
                do
                {
                    node = this.Fish();

                    // Ok we've tried 100 times to find a recipe not already in this set, there just isn't enough data to work with for this query
                    if (++timeout > 100) 
                    {
                        throw new ImpossibleQueryException(); // TODO: Maybe we can lower the demanded meals and return what we do have
                    }
                } while (inUse.Contains(node.RecipeId));

                inUse.Add(node.RecipeId);
                currentSet[i] = node;
            }
        }

        /// <summary>
        /// Swap out a random recipe with another one from the available pool
        /// </summary>
        private RecipeNode[] GetNextSet(RecipeNode[] currentSet)
        {
            var randomIndex = this.random.Next(currentSet.Length);
            var existingRecipe = currentSet[randomIndex];
            RecipeNode newRecipe;

            int timeout = 0;
            while (true)
            {
                // We've tried 100 times to replace a recipe in this set, but cannot find anything that isn't already in this set.
                if (++timeout > 100) 
                {
                    throw new ImpossibleQueryException(); // TODO: If this is the only set of n which match that query, we've solved it - just return this set as final!
                }

                newRecipe = this.Fish();
                if (newRecipe == existingRecipe)
                {
                    continue;
                }

                bool fFound = false;
                for (var i = 0; i < currentSet.Length; i++)
                {
                    if (newRecipe == currentSet[i])
                    {
                        fFound = true;
                        break;
                    }
                }

                if (!fFound)
                {
                    break;
                }
            }

            var resultSet = new RecipeNode[currentSet.Length];
            currentSet.CopyTo(resultSet, 0);
            resultSet[randomIndex] = newRecipe;

            return resultSet;
        }

        /// <summary>
        /// Finds a random recipe in the available recipe pool
        /// </summary>
        private RecipeNode Fish()
        {
            RecipeNode recipeNode;

            // No pantry, fish through Recipe Index
            if (this.pantryIngredients == null) 
            {
                int rnd;
                var tag = (this.allowedTags == null) ? this.random.Next(RecipeTag.NumberOfTags) : this.allowedTags[this.random.Next(this.allowedTags.Length)].Value;
                var recipesByTag = this.dataBase.FindRecipesByTag(tag);
                if (recipesByTag == null || recipesByTag.Length == 0)
                {
                    return this.Fish();
                }

                rnd = this.random.Next(recipesByTag.Length);
                recipeNode = recipesByTag[rnd];
            }
            else 
            {
                // Fish through random pantry ingredient
                int randomIngredient = this.random.Next(this.pantryIngredients.Length);
                var ingredientNode = this.pantryIngredients[randomIngredient];

                RecipeNode[] recipes;
                if (this.allowedTags != null && this.allowedTags.Length > 0)
                {
                    // Does this ingredient have any allowed tags?
                    if ((this.allowedTags & ingredientNode.AvailableTags) == 0) 
                    {
                        return this.Fish(); // No - Find something else
                    }

                    // Pick random tag from allowed tags (since this set is smaller, better to guess an available tag)
                    while (true)
                    {
                        int tag = this.random.Next(this.allowedTags.Length); // NOTE: Next will NEVER return MaxValue, so we don't subtract 1 from Length!
                        var randomTag = this.allowedTags[tag];
                        recipes = ingredientNode.RecipesByTag[randomTag.Value] as RecipeNode[];

                        if (recipes != null)
                        {
                            break;
                        }
                    }
                }
                else 
                {
                    // Just pick a random available tag
                    int randomTag = this.random.Next(ingredientNode.AvailableTags.Length);
                    var tag = ingredientNode.AvailableTags[randomTag];
                    recipes = ingredientNode.RecipesByTag[tag.Value] as RecipeNode[];
                }

                int randomRecipe = this.random.Next(recipes.Length);
                recipeNode = recipes[randomRecipe];
            }

            // If there's a blacklist, make sure no ingredients are blacklisted otherwise try again
            if (this.ingredientBlacklist != null)
            {
                var ingredients = (IngredientUsage[])recipeNode.Ingredients;
                for (int i = 0; i < ingredients.Length; i++)
                {
                    if (this.ingredientBlacklist.Contains(ingredients[i].Ingredient))
                    {
                        return this.Fish();
                    }
                }
            }

            // Discard if this recipe is to be avoided
            if (this.profile.AvoidRecipe.HasValue && this.profile.AvoidRecipe.Value.Equals(recipeNode.RecipeId))
            {
                return this.Fish();
            }

            return recipeNode;
        }
    }
}