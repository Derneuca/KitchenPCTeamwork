namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Serialization;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Data;
    using KitchenPC.Data.DTO;
    using KitchenPC.Fluent.Menus;
    using KitchenPC.Fluent.Modeler;
    using KitchenPC.Fluent.Queue;
    using KitchenPC.Fluent.Recipes;
    using KitchenPC.Fluent.ShoppingLists;
    using KitchenPC.Ingredients;
    using KitchenPC.Menus;
    using KitchenPC.Modeler;
    using KitchenPC.NLP;
    using KitchenPC.Provisioning;
    using KitchenPC.Recipes;
    using KitchenPC.Recipes.Enums;
    using KitchenPC.ShoppingLists;
    using IngredientUsage = KitchenPC.Ingredients.IngredientUsage;

    public class StaticContext : IKPCContext, IProvisionTarget, IProvisionSource
    {
        private readonly StaticContextBuilder builder;

        private DataStore store;
        private IngredientParser ingredientParser;
        private ISearchProvider searchProvider;

        public StaticContext()
        {
            this.builder = new StaticContextBuilder(this);
        }

        /// <summary>
        /// Gets a StaticContextBuilder used to configure a StaticContext instance.
        /// </summary>
        public static StaticContextBuilder Configure
        {
            get
            {
                return new StaticContext().builder;
            }
        }

        public string DataDirectory { get; set; }

        public bool CompressedStore { get; set; }

        public Func<AuthIdentity> GetIdentity { get; set; }

        public Parser Parser { get; private set; }

        public ModelerProxy ModelerProxy { get; private set; }

        /// <summary>
        /// Gets the identity of the current user using the GetIdentity function.
        /// </summary>
        public AuthIdentity Identity
        {
            get
            {
                return this.GetIdentity();
            }
        }

        /// <summary>
        /// Gets an object able to load modeling information.  This will be called automatically when the modeler is initialized.
        /// </summary>
        public IModelerLoader ModelerLoader
        {
            get
            {
                return new StaticModelerLoader(this.store);
            }
        }

        /// <summary>Provides the ability to fluently work with menus.</summary>
        public MenuAction Menus
        {
            get
            {
                return new MenuAction(this);
            }
        }

        /// <summary>Provides the ability to fluently work with recipes.</summary>
        public RecipeAction Recipes
        {
            get
            {
                return new RecipeAction(this);
            }
        }

        /// <summary>Provides the ability to fluently work with shopping lists.</summary>
        public ShoppingListAction ShoppingLists
        {
            get
            {
                return new ShoppingListAction(this);
            }
        }

        /// <summary>Provides the ability to fluently work with the recipe queue.</summary>
        public QueueAction Queue
        {
            get
            {
                return new QueueAction(this);
            }
        }

        /// <summary>Provides the ability to fluently work with the recipe modeler.</summary>
        public ModelerAction Modeler
        {
            get
            {
                return new ModelerAction(this);
            }
        }

        /// <summary>
        /// Initializes the context and loads necessary data into memory through the configured data file.
        /// This must be done before the context is usable.
        /// </summary>
        public void Initialize()
        {
            var file = this.CompressedStore ? "KPCData.gz" : "KPCData.xml";
            var path = Path.Combine(this.DataDirectory, file);
            KPCContext.Log.DebugFormat("Attempting to open local data file: {0}", path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Could not initialize StaticContext.  Data file not found.", path);
            }

            var serializer = new XmlSerializer(typeof(DataStore));
            using (var fileReader = new FileStream(path, FileMode.Open))
            {
                if (this.CompressedStore)
                {
                    using (var reader = new GZipStream(fileReader, CompressionMode.Decompress))
                    {
                        this.store = serializer.Deserialize(reader) as DataStore;
                    }
                }
                else
                {
                    this.store = serializer.Deserialize(fileReader) as DataStore;
                }

                if (this.store == null)
                {
                    throw new DataStoreException("Could not deserialize data store.  It might be correct or an invalid format.");
                }
            }

            // Initialize ingredient parser
            this.ingredientParser = new IngredientParser();
            this.ingredientParser.CreateIndex(this.store.Ingredients.Select(i => new IngredientSource(i.IngredientId, i.DisplayName)));

            // Initialize modeler
            this.ModelerProxy = new ModelerProxy(this);
            this.ModelerProxy.LoadSnapshot();

            // Initialize natural language parsing
            IngredientSynonyms.InitIndex(new StaticIngredientLoader(this.store));
            UnitSynonyms.InitIndex(new StaticUnitLoader(this.store));
            FormSynonyms.InitIndex(new StaticFormLoader(this.store));
            PrepNotes.InitIndex(new StaticPrepLoader(this.store));
            Anomalies.InitIndex(new StaticAnomalyLoader(this.store));
            NumericVocab.InitIndex();

            this.Parser = new Parser();
            this.LoadTemplates();
        }

        /// <summary>
        /// Takes part of an ingredient name and returns possible matches, useful for autocomplete UIs.
        /// </summary>
        /// <param name="query">Part or all of an ingredient name.  Must be at least three characters.</param>
        /// <returns>An enumeration of IngredientNode objects describing possible matches and their IDs.</returns>
        public IEnumerable<IngredientNode> AutocompleteIngredient(string query)
        {
            return this.ingredientParser.MatchIngredient(query);
        }

        /// <summary>
        /// Creates a new recipe modeling session.  Recipe modeling allows the user to generate optimal sets of recipes based on given ingredient usage and criteria.
        /// </summary>
        /// <param name="profile">A profile for the current user.  Pass in UserProfile.Anonymous to indicate a generic user.</param>
        /// <returns>A modeling session able to generate and compile recipe sets based on the given profile.</returns>
        public ModelingSession CreateModelingSession(IUserProfile profile)
        {
            return this.ModelerProxy.CreateSession(profile);
        }

        /// <summary>
        /// Attempts to parse an ingredient usage using natural language processing (NLP).
        /// </summary>
        /// <param name="input">An ingredient usage, such as "2 eggs" or "1/4 cup of shredded cheese"</param>
        /// <returns>A Result object indicating if the usage could be parsed, and if so, the normalized ingredient usage information.</returns>
        public Result ParseIngredientUsage(string input)
        {
            return Parser.Parse(input);
        }

        /// <summary>
        /// Attempts to parse an ingredient by name using natural language processing (NLP).  A single ingredient might have various synonyms, spellings, etc.
        /// </summary>
        /// <param name="input">The name of an ingredient.</param>
        /// <returns>A KitchenPC Ingredient object, or null if no matching ingredient was found.</returns>
        public Ingredient ParseIngredient(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var result = this.ParseIngredientUsage(input.Trim());
            if (result is Match)
            {
                return result.Usage.Ingredient;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches for recipes matching the specified criteria.
        /// </summary>
        /// <param name="query">A RecipeQuery object indicating the recipes to match.</param>
        /// <returns>A SearchResults object containing recipe briefs and a total count.</returns>
        public SearchResults RecipeSearch(RecipeQuery query)
        {
            if (this.searchProvider == null)
            {
                this.searchProvider = new StaticSearch(this.store);
            }

            return this.searchProvider.Search(this.Identity, query);
        }

        /// <summary>
        /// Reads full information for one or more recipes in the database.
        /// </summary>
        /// <param name="recipeIds">An array containing recipe IDs to load.</param>
        /// <param name="options">Indicates which properties to load.  Use ReadRecipeOptions.None to only load base recipe data.</param>
        /// <returns></returns>
        public Recipe[] ReadRecipes(Guid[] recipeIds, ReadRecipeOptions options)
        {
            var recipes = this.store.Recipes.Where(r => recipeIds.Contains(r.RecipeId)).ToList();

            if (!recipes.Any())
            {
                throw new RecipeNotFoundException();
            }

            var indexedRecipeIngredients = this.store.GetIndexedRecipeIngredients();
            var indexedIngredients = this.store.GetIndexedIngredients();
            var indexedIngredientForms = this.store.GetIndexedIngredientForms();
            var indexedRecipeMetadata = this.store.GetIndexedRecipeMetadata();
            var indexedIngredientMetadata = this.store.GetIndexedIngredientMetadata();

            var result = new List<Recipe>();
            foreach (var r in recipes)
            {
                var recipe = new Recipe
                {
                    Id = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                    DateEntered = r.DateEntered,
                    ImageUrl = r.ImageUrl,
                    ServingSize = r.ServingSize,
                    PreparationTime = r.PreparationTime,
                    CookTime = r.CookTime,
                    Credit = r.Credit,
                    CreditUrl = r.CreditUrl,
                    AvgRating = r.Rating
                };

                if (options.ReturnMethod)
                {
                    recipe.Method = r.Steps;
                }

                if (options.ReturnUserRating)
                {
                    var userRating = this.store.RecipeRatings.SingleOrDefault(ur => (ur.RecipeId == r.RecipeId && ur.UserId == this.Identity.UserId));
                    recipe.UserRating = userRating != null ? (Rating)userRating.Rating : Rating.None;
                }

                recipe.Ingredients = indexedRecipeIngredients[r.RecipeId].Select(i => new IngredientUsage
                {
                    Amount = i.Qty.HasValue ? new Amount(i.Qty.Value, i.Unit) : null,
                    PreparationNote = i.PreparationNote,
                    Section = i.Section,
                    Form = i.IngredientFormId.HasValue ? IngredientForms.ToIngredientForm(indexedIngredientForms[i.IngredientFormId.Value]) : null,
                    Ingredient = Data.DTO.Ingredients.ToIngredient(indexedIngredients[i.IngredientId], indexedIngredientMetadata[i.IngredientId])
                }).ToArray();

                recipe.Tags = RecipeMetadata.ToRecipeTags(indexedRecipeMetadata[r.RecipeId]);
                result.Add(recipe);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Associates a rating with the current user and a specified recipe.
        /// </summary>
        /// <param name="recipeId">Recipe ID to rate</param>
        /// <param name="rating">Rating to give this recipe</param>
        public void RateRecipe(Guid recipeId, Rating rating)
        {
            var userRating = this.store.RecipeRatings.SingleOrDefault(ur => (ur.RecipeId == recipeId && ur.UserId == this.Identity.UserId));
            if (userRating == null)
            {
                this.store.RecipeRatings.Add(new RecipeRatings
                {
                    RatingId = Guid.NewGuid(),
                    RecipeId = recipeId,
                    UserId = this.Identity.UserId,
                    Rating = (short)rating
                });
            }
            else
            {
                userRating.Rating = (short)rating;
            }
        }

        /// <summary>
        /// Creates a new recipe.
        /// </summary>
        /// <param name="recipe">Fully constructed Recipe object.</param>
        public RecipeResult CreateRecipe(Recipe recipe)
        {
            // Recipe.Validate(recipe); ----> Validate() removed by PsychoSphere 
            recipe.Id = Guid.NewGuid();

            // TODO: We should update indexes rather than clear them all out, however this context isn't designed for performance
            this.store.ClearIndexes();

            // Recipes
            this.store.Recipes.Add(new Data.DTO.Recipes
            {
                RecipeId = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                CookTime = recipe.CookTime,
                PreparationTime = recipe.PreparationTime,
                Credit = recipe.Credit,
                CreditUrl = recipe.CreditUrl,
                DateEntered = recipe.DateEntered,
                ImageUrl = recipe.ImageUrl,
                Rating = recipe.AvgRating,
                ServingSize = recipe.ServingSize,
                Steps = recipe.Method
            });

            // RecipeIngredients
            short displayOrder = 0;
            recipe.Ingredients.ForEach(i =>
            {
                store.RecipeIngredients.Add(new RecipeIngredients
                {
                    RecipeIngredientId = Guid.NewGuid(),
                    RecipeId = recipe.Id,
                    IngredientId = i.Ingredient.Id,
                    IngredientFormId = i.Form != null ? (Guid?)i.Form.FormId : null,
                    Qty = i.Amount != null ? (float?)i.Amount.SizeHigh : null,
                    QtyLow = i.Amount != null ? (float?)i.Amount.SizeLow : null,
                    Unit = i.Amount != null ? i.Amount.Unit : Units.Unit,
                    Section = i.Section,
                    DisplayOrder = ++displayOrder
                });
            });

            // RecipeMetadata
            this.store.RecipeMetadata.Add(new RecipeMetadata
            {
                RecipeMetadataId = Guid.NewGuid(),
                RecipeId = recipe.Id,
                DietGlutenFree = recipe.Tags.HasTag(RecipeTag.GlutenFree),
                DietNoAnimals = recipe.Tags.HasTag(RecipeTag.NoAnimals),
                DietNomeat = recipe.Tags.HasTag(RecipeTag.NoMeat),
                DietNoPork = recipe.Tags.HasTag(RecipeTag.NoPork),
                DietNoRedMeat = recipe.Tags.HasTag(RecipeTag.NoRedMeat),
                MealBreakfast = recipe.Tags.HasTag(RecipeTag.Breakfast),
                MealDessert = recipe.Tags.HasTag(RecipeTag.Dessert),
                MealDinner = recipe.Tags.HasTag(RecipeTag.Dinner),
                MealLunch = recipe.Tags.HasTag(RecipeTag.Lunch),
                NutritionLowCalorie = recipe.Tags.HasTag(RecipeTag.LowCalorie),
                NutritionLowCarb = recipe.Tags.HasTag(RecipeTag.LowCarb),
                NutritionLowFat = recipe.Tags.HasTag(RecipeTag.LowFat),
                NutritionLowSodium = recipe.Tags.HasTag(RecipeTag.LowSodium),
                NutritionLowSugar = recipe.Tags.HasTag(RecipeTag.LowSugar),
                SkillCommonIngredients = recipe.Tags.HasTag(RecipeTag.CommonIngredients),
                SkillEasyToMake = recipe.Tags.HasTag(RecipeTag.EasyToMake),
                SkillQuick = recipe.Tags.HasTag(RecipeTag.Quick)
            });

            return new RecipeResult
            {
                RecipeCreated = true,
                NewRecipeId = recipe.Id
            };
        }

        /// <summary>
        /// Removes one or more recipes from the user's recipe queue.
        /// </summary>
        /// <param name="recipeIds">A list of recipe IDs to remove from the queue.  IDs not in the queue will be ignored.</param>
        public void DequeueRecipe(params Guid[] recipeIds)
        {
            var existing = this.store.QueuedRecipes
               .Where(r => r.UserId == this.Identity.UserId)
               .Where(r => recipeIds.Contains(r.RecipeId));

            this.store.QueuedRecipes.RemoveAll(existing.Contains);
        }

        /// <summary>
        /// Adds one or more recipes to the user's recipe queue.
        /// </summary>
        /// <param name="recipeIds">A list of recipe IDs to add to the queue.  IDs already in the queue will be ignored.</param>
        public void EnqueueRecipes(params Guid[] recipeIds)
        {
            var existing = this.store.QueuedRecipes
               .Where(r => r.UserId == this.Identity.UserId)
               .Where(r => recipeIds.Contains(r.RecipeId))
               .Select(r => r.RecipeId);

            var noDupes = recipeIds.Except(existing).ToList();
            if (!noDupes.Any())
            {
                return;
            }

            this.store.QueuedRecipes.AddRange(noDupes.Select(r => new QueuedRecipes
            {
                QueueId = Guid.NewGuid(),
                QueuedDate = DateTime.Now,
                UserId = this.Identity.UserId,
                RecipeId = r
            }));
        }

        /// <summary>
        /// Returns the user's current recipe queue.
        /// </summary>
        /// <returns>An array of RecipeBrief objects for each recipe in the queue</returns>
        public RecipeBrief[] GetRecipeQueue()
        {
            var indexedRecipes = this.store.GetIndexedRecipes();
            var queue = this.store.QueuedRecipes.Where(q => q.UserId == this.Identity.UserId);

            return queue
               .Select(item => indexedRecipes[item.RecipeId])
               .Select(Data.DTO.Recipes.ToRecipeBrief)
               .ToArray();
        }

        /// <summary>
        /// Reads the available forms for the given ingredient ID.  Forms indicate ways an ingredient might be used within a recipe, such as "chopped", "sliced" or "melted".
        /// </summary>
        /// <param name="id">An ingredient ID</param>
        /// <returns>An IngredientFormsCollection object containing an array of ingredient forms.</returns>
        public IngredientFormsCollection ReadFormsForIngredient(Guid id)
        {
            var forms = this.store.IngredientForms
               .Where(f => f.IngredientId == id)
               .Select(IngredientForms.ToIngredientForm);

            return new IngredientFormsCollection(forms);
        }

        /// <summary>
        /// Returns ingredient information, such as ID, metadata, unit information, etc.
        /// </summary>
        /// <param name="ingredient">The name of an ingredient.  This must be an exact match.</param>
        /// <returns>A KitchenPC Ingredient object, or null if no matching ingredient was found.</returns>
        public Ingredient ReadIngredient(string ingredient)
        {
            var ing =
                this.store.Ingredients
                .FirstOrDefault(i => string.Compare(
                    i.DisplayName,
                    ingredient,
                    StringComparison.OrdinalIgnoreCase) == 0);

            var result = ing == null ? null : Data.DTO.Ingredients.ToIngredient(ing);
            return result;
        }

        /// <summary>
        /// Returns ingredient information, such as ID, metadata, unit information, etc.
        /// </summary>
        /// <param name="ingredientId">The ID of the ingredient.</param>
        /// <returns>A KitchenPC Ingredient object, or null if no matching ingredient was found.</returns>
        public Ingredient ReadIngredient(Guid ingredientId)
        {
            var indexedIngredients = this.store.GetIndexedIngredients();
            Data.DTO.Ingredients dtoIngredient;

            var result =
                indexedIngredients.TryGetValue(ingredientId, out dtoIngredient)
                ? Data.DTO.Ingredients.ToIngredient(dtoIngredient)
                : null;
            return result;
        }

        /// <summary>
        /// Converts a usage of an ingredient within a recipe to an IngredientAggregation object, suitable for aggregating with other usages of the same ingredient.
        /// </summary>
        /// <param name="usage">An IngredientUsage object, usually from a recipe.</param>
        /// <returns>An IngredientAggregation object, usually to be combined with other uses of that ingredient to form a shopping list.</returns>
        public IngredientAggregation ConvertIngredientUsage(IngredientUsage usage)
        {
            // TODO: Does this method need to be part of the context?  Perhaps IngredientUsage should have a method to convert to an aggregation
            var ingredient = this.ReadIngredient(usage.Ingredient.Id);
            if (ingredient == null)
            {
                throw new IngredientNotFoundException();
            }

            var aggregation = new IngredientAggregation(ingredient);
            aggregation.AddUsage(usage);

            return aggregation;
        }

        /// <summary>
        /// Returns one or more saved shopping lists from the current user.
        /// </summary>
        /// <param name="lists">A list of ShoppingList objects indicating the ID of the list to load, or ShoppingList.Default for the default list.</param>
        /// <param name="options">Indicates what data to load.  Use GetShoppingListOptions.None to simply load the names of the lists.</param>
        /// <returns>An array of ShoppingList objects with all the desired properties loaded.</returns>
        public ShoppingList[] GetShoppingLists(IList<ShoppingList> lists, GetShoppingListOptions options)
        {
            bool loadDefault = true;
            var query = this.store.ShoppingLists
               .Where(p => p.UserId == this.Identity.UserId);

            if (lists != null)
            {
                // Load individual lists
                loadDefault = lists.Contains(ShoppingList.Default);
                var ids = lists.Where(list => list.Id.HasValue).Select(l => l.Id.Value).ToArray();
                query = query.Where(p => ids.Contains(p.ShoppingListId));
            }

            var dbLists = query.ToList();

            if (!options.LoadItems)
            {
                return (loadDefault ? new ShoppingList[] { ShoppingList.Default } : new ShoppingList[0])
                   .Concat(dbLists.Select(Data.DTO.ShoppingLists.ToShoppingList))
                   .ToArray();
            }

            // All user's shopping list items
            var dbItems = this.store.ShoppingListItems
               .Where(p => p.UserId == this.Identity.UserId)
               .ToList();

            var indexedRecipes = this.store.GetIndexedRecipes();
            var indexedIngredients = this.store.GetIndexedIngredients();

            var result = new List<ShoppingList>();
            if (loadDefault)
            {
                result.Add(ShoppingList.Default);
            }

            result.AddRange(dbLists.Select(Data.DTO.ShoppingLists.ToShoppingList));

            // Add items to list
            foreach (var list in result)
            {
                var itemsInList = dbItems.Where(p => p.ShoppingListId.Equals(list.Id));
                var items = itemsInList.Select(item =>
                    new ShoppingListItem(item.ItemId)
                    {
                        Raw = item.Raw,
                        Ingredient =
                            item.IngredientId.HasValue
                            ? Data.DTO.Ingredients.ToIngredient(indexedIngredients[item.IngredientId.Value])
                            : null,
                        Recipe =
                            item.RecipeId.HasValue
                            ? Data.DTO.Recipes.ToRecipeBrief(indexedRecipes[item.RecipeId.Value])
                            : null,
                        CrossedOut = item.CrossedOut,
                        Amount =
                            item.Qty.HasValue && item.Unit.HasValue
                            ? new Amount(item.Qty.Value, item.Unit.Value)
                            : null
                    });

                list.AddItems(items.ToList());
            }

            return result.ToArray();
        }

        /// <summary>
        /// Creates a new shopping list for the current user.
        /// </summary>
        /// <param name="name">The name of the new shopping list.</param>
        /// <param name="recipes">Zero or more recipes to add to this list.</param>
        /// <param name="ingredients">Zero or more ingredients to add to this list.</param>
        /// <param name="usages">Zero or more ingredient usages to add to this list.</param>
        /// <param name="items">Zero or more raw usages.  Raw usages will be parsed using NLP, and unsuccessful matches will be added to the list as raw items.</param>
        /// <returns>A fully aggregated shopping list, with like items combined and forms normalized.</returns>
        public ShoppingListResult CreateShoppingList(
            string name,
            Recipe[] recipes,
            Ingredient[] ingredients,
            IngredientUsage[] usages,
            string[] items)
        {
            var parsedIngredients = this.Parser.ParseAll(items).ToList();

            var recipeAggregation = this.AggregateRecipes(recipes.Select(r => r.Id).ToArray());
            var ingredientAggregation = ingredients.Select(i => new IngredientAggregation(i, null));
            var ingredientUsages = this.AggregateIngredients(usages);
            var parsedUsages = this.AggregateIngredients(parsedIngredients.Where(u => u is Match).Select(u => u.Usage).ToArray());
            var rawInputs = parsedIngredients.Where(u => u is NoMatch).Select(u => new ShoppingListItem(u.Input));

            var allItems = recipeAggregation
                .Concat(ingredientAggregation)
                .Concat(ingredientUsages)
                .Concat(parsedUsages)
                .Concat(rawInputs);

            var list = new ShoppingList(null, name, allItems);
            return this.CreateShoppingList(list);
        }

        /// <summary>
        /// Creates a new shopping list for the current user.
        /// </summary>
        /// <param name="list">A ShoppingList object containing a normalized shopping list.</param>
        /// <returns>A result indicating the ID assigned to the newly created list.</returns>
        public ShoppingListResult CreateShoppingList(ShoppingList list)
        {
            var result = new ShoppingListResult();
            var dbList = new Data.DTO.ShoppingLists
            {
                ShoppingListId = Guid.NewGuid(),
                Title = list.Title.Trim(),
                UserId = this.Identity.UserId
            };

            this.store.ShoppingLists.Add(dbList);

            if (list.Any())
            {
                // Create ShoppingListItems
                list.ToList().ForEach(i => this.store.ShoppingListItems.Add(new ShoppingListItems
                {
                    ItemId = Guid.NewGuid(),
                    UserId = this.Identity.UserId,
                    ShoppingListId = dbList.ShoppingListId,
                    CrossedOut = i.CrossedOut,
                    IngredientId = i.Ingredient != null ? (Guid?)i.Ingredient.Id : null,
                    RecipeId = i.Recipe != null ? (Guid?)i.Recipe.Id : null,
                    Raw = i.Raw,
                    Qty = i.Amount != null ? (float?)i.Amount.SizeHigh : null,
                    Unit = i.Amount != null ? (Units?)i.Amount.Unit : null
                }));
            }

            result.NewShoppingListId = dbList.ShoppingListId;
            result.List = list;
            return result;
        }

        /// <summary>
        /// Updates a shopping list.
        /// </summary>
        /// <param name="list">A shopping list owned by the current user.</param>
        /// <param name="updates">A set of update commands indicating how the shopping list should be updated.</param>
        /// <param name="newName">An optional new name for this shopping list.</param>
        /// <returns></returns>
        public ShoppingListResult UpdateShoppingList(ShoppingList list, ShoppingListUpdateCommand[] updates, string newName = null)
        {
            // Aggregate new items
            var parsedIngredients = this.Parser.ParseAll(updates.Where(u => !string.IsNullOrWhiteSpace(u.NewRaw)).Select(r => r.NewRaw).ToArray()).ToList();

            var recipeAggregation = this.AggregateRecipes(updates
                .Where(u => u.NewRecipe != null)
                .Select(r => r.NewRecipe.Id).ToArray());

            var ingredientAggregation = updates
                .Where(u => u.NewIngredient != null)
                .Select(i => new IngredientAggregation(i.NewIngredient, null));

            var ingredientUsages = this.AggregateIngredients(updates
                .Where(u => u.NewUsage != null)
                .Select(u => u.NewUsage).ToArray());

            var parsedUsages = this.AggregateIngredients(parsedIngredients
                .Where(u => u is Match)
                .Select(u => u.Usage).ToArray());

            var rawInputs = parsedIngredients
                .Where(u => u is NoMatch)
                .Select(u => new ShoppingListItem(u.Input));

            var newItems = recipeAggregation
                .Concat(ingredientAggregation)
                .Concat(ingredientUsages)
                .Concat(parsedUsages)
                .Concat(rawInputs);

            var removedItems = updates
                .Where(u => u.Command == ShoppingListUpdateType.RemoveItem)
                .Select(i => i.RemoveItem.Value).ToArray();
            var modifiedItems = updates
                .Where(u => u.Command == ShoppingListUpdateType.ModifyItem)
                .Select(i => i.ModifyItem);

            return this.UpdateShoppingList(list.Id, removedItems, modifiedItems, newItems, newName);
        }

        /// <summary>
        /// Aggregates one or more recipes into a set of aggregated ingredients.  This is normally used to create a list of items needed to buy for multiple recipes without having to create a shopping list.
        /// </summary>
        /// <param name="recipeIds">A list of recipe IDs to aggregate.</param>
        /// <returns>A list of IngredientAggregation objects, one per unique ingredient in the set of recipes</returns>
        public IList<IngredientAggregation> AggregateRecipes(params Guid[] recipeIds)
        {
            var ingredients = new Dictionary<Guid, IngredientAggregation>(); // List of all ingredients and total usage

            foreach (var id in recipeIds)
            {
                // Lookup ingredient through modeler cache
                var recipeNode = ModelerProxy.FindRecipe(id);
                if (recipeNode == null)
                {
                    // Our cache is out of date, skip this result
                    continue;
                }

                foreach (var usage in recipeNode.Ingredients)
                {
                    var ingredientId = usage.Ingredient.IngredientId;
                    string ingredientName = this.ingredientParser.GetIngredientById(ingredientId);
                    var ingredient = new Ingredient(ingredientId, ingredientName);
                    ingredient.ConversionType = usage.Ingredient.ConversionType;

                    IngredientAggregation aggregation;
                    if (!ingredients.TryGetValue(ingredientId, out aggregation))
                    {
                        aggregation = new IngredientAggregation(ingredient)
                        {
                            Amount = new Amount(0, usage.Unit)
                        };

                        ingredients.Add(ingredientId, aggregation);
                    }

                    // TODO: If usage.Unit is different than agg.Amount.Unit then we have a problem, throw an exception if that happens?
                    if (aggregation.Amount == null)
                    {
                        // This aggregation contains an empty amount, so we can't aggregate
                        continue;
                    }
                    else if (!usage.Amount.HasValue)
                    {
                        // This amount is null, cancel aggregation
                        aggregation.Amount = null;
                    }
                    else
                    {
                        aggregation.Amount += usage.Amount.Value;
                    }
                }
            }

            return ingredients.Values.ToList();
        }

        /// <summary>
        /// Aggregates one or more IngredientUsage objects.
        /// </summary>
        /// <param name="usages">IngredientUsage objects, usually from one or more recipes</param>
        /// <returns>A list of IngredientAggregation objects, one per unique ingredient in the set of recipes</returns>
        public IList<IngredientAggregation> AggregateIngredients(params IngredientUsage[] usages)
        {
            var ingredients = new Dictionary<Guid, IngredientAggregation>();

            foreach (var usage in usages)
            {
                IngredientAggregation aggregation;

                if (!ingredients.TryGetValue(usage.Ingredient.Id, out aggregation))
                {
                    ingredients.Add(usage.Ingredient.Id, new IngredientAggregation(usage.Ingredient, usage.Amount));
                    continue;
                }

                // TODO: If usage.Unit is different than agg.Amount.Unit then we have a problem, throw an exception if that happens?
                if (aggregation.Amount == null)
                {
                    // This aggregation contains an empty amount, so we can't aggregate
                    continue;
                }
                else if (usage.Amount == null)
                {
                    // This amount is null, cancel aggregation
                    aggregation.Amount = null;
                }
                else
                {
                    aggregation.Amount += usage.Amount;
                }
            }

            return ingredients.Values.ToList();
        }

        /// <summary>
        /// Deletes one or more shopping lists owned by the current user.
        /// </summary>
        /// <param name="lists">One or more shopping lists to delete.  Note, the default shopping list cannot be deleted.</param>
        public void DeleteShoppingLists(ShoppingList[] lists)
        {
            var ids = lists.Where(p => p.Id.HasValue).Select(p => p.Id.Value).ToList();

            var dbItems = this.store.ShoppingListItems
                .Where(p => p.UserId == this.Identity.UserId)
                .Where(p => p.ShoppingListId.HasValue)
                .Where(p => ids.Contains(p.ShoppingListId.Value));

            var dbLists = this.store.ShoppingLists
                .Where(p => p.UserId == this.Identity.UserId)
                .Where(p => ids.Contains(p.ShoppingListId));

            this.store.ShoppingListItems.RemoveAll(dbItems.Contains);
            this.store.ShoppingLists.RemoveAll(dbLists.Contains);
        }

        /// <summary>
        /// Returns the specified set of menus owned by the current user.
        /// </summary>
        /// <param name="menus">One or more Menu objects.  Use Menu.Favorites to load the default favorites menu.</param>
        /// <param name="options">Specifies what data to load.</param>
        /// <returns>An array of Menu objects with the specified data loaded.</returns>
        public Menu[] GetMenus(IList<Menu> menus, GetMenuOptions options)
        {
            bool areFavoritesLoaded = true;
            var query = this.store.Menus.Where(p => p.UserId == this.Identity.UserId);

            if (menus != null)
            {
                // Load individual menus
                areFavoritesLoaded = menus.Contains(Menu.Favorites);
                var ids = menus.Where(m => m.Id.HasValue).Select(m => m.Id.Value);
                query = query.Where(p => ids.Contains(p.MenuId));
            }

            var dbMenus = query.ToList();
            var result = new List<Menu>();

            if (areFavoritesLoaded)
            {
                result.Add(Menu.Favorites);
            }

            result.AddRange(dbMenus.Select(Data.DTO.Menus.ToMenu));

            if (!options.LoadRecipes)
            {
                return result.ToArray();
            }

            var indexedRecipes = this.store.GetIndexedRecipes();
            var dbFavorites = this.store.Favorites.Where(p => p.UserId == this.Identity.UserId);

            return result.Select(m =>
               new Menu(m)
               {
                   Recipes = (m.Id.HasValue
                      ? dbFavorites.Where(f => f.MenuId.HasValue && f.MenuId == m.Id)
                      : dbFavorites.Where(f => f.MenuId == null))
                      .Select(r => Data.DTO.Recipes.ToRecipeBrief(indexedRecipes[r.RecipeId]))
                      .ToArray()
               }).ToArray();
        }

        /// <summary>
        /// Deletes one or more menus owned by the current user.
        /// </summary>
        /// <param name="menuIds">One or more menus to delete.  Note, the Favorites menu cannot be deleted.</param>
        public void DeleteMenus(params Guid[] menuIds)
        {
            var dbFavorites = this.store.Favorites
                .Where(p => p.UserId == this.Identity.UserId)
                .Where(p => p.MenuId.HasValue)
                .Where(p => menuIds.Contains(p.MenuId.Value));

            var dbMenus = this.store.Menus
                .Where(p => p.UserId == this.Identity.UserId)
                .Where(p => menuIds.Contains(p.MenuId));

            this.store.Favorites.RemoveAll(dbFavorites.Contains);
            this.store.Menus.RemoveAll(dbMenus.Contains);
        }

        /// <summary>
        /// Updates a specified menu owned by the current user.
        /// </summary>
        /// <param name="menuId">The Menu ID to update, or null to update the Favorites menu.</param>
        /// <param name="recipesAdd">A list of recipe IDs to add to the menu.  Duplicates will be ignored.</param>
        /// <param name="recipesRemove">A list of recipe IDs to remove from the menu.</param>
        /// <param name="recipesMove">A list of items to move from this menu to another menu.</param>
        /// <param name="clear">If true, all recipes will be removed from this menu.</param>
        /// <param name="newName">An optional new name for this menu.  Note, the favorites menu cannot be renamed.</param>
        /// <returns></returns>
        public MenuResult UpdateMenu(
            Guid? menuId,
            Guid[] recipesAdd,
            Guid[] recipesRemove,
            MenuMove[] recipesMove,
            bool clear,
            string newName = null)
        {
            var result = new MenuResult();
            result.MenuUpdated = true; // TODO: Verify actual changes were made before setting MenuUpdated to true

            Data.DTO.Menus dbMenu = null;
            if (menuId.HasValue)
            {
                dbMenu = this.store.Menus.SingleOrDefault(p => p.MenuId == menuId);
                if (dbMenu == null)
                {
                    throw new MenuNotFoundException();
                }

                if (dbMenu.UserId != this.Identity.UserId)
                {
                    // User does not have access to modify this menu
                    throw new UserDoesNotOwnMenuException();
                }
            }

            var dbFavorites = this.store.Favorites
                .Where(p => p.MenuId == menuId)
                .ToList();

            if (!string.IsNullOrWhiteSpace(newName) && dbMenu != null)
            {
                // Rename menu
                dbMenu.Title = newName.Trim();
            }

            if (recipesAdd.Any())
            {
                // Add recipes to menu
                var existing = dbFavorites.Select(f => f.RecipeId);
                recipesAdd = recipesAdd.Except(existing).ToArray(); // Remove dupes

                foreach (var recipeId in recipesAdd)
                {
                    var favorite = new Favorites
                    {
                        FavoriteId = Guid.NewGuid(),
                        UserId = this.Identity.UserId,
                        RecipeId = recipeId,
                        MenuId = menuId
                    };

                    this.store.Favorites.Add(favorite);
                }
            }

            if (recipesRemove.Any())
            {
                // Remove recipes from menu
                var toDelete =
                    from r in dbFavorites
                    where recipesRemove.Contains(r.RecipeId)
                    select r;
                toDelete.ForEach(r => this.store.Favorites.Remove(r));
            }

            if (clear)
            {
                // Remove every recipe from menu
                this.store.Favorites.RemoveAll(dbFavorites.Contains);
            }

            if (recipesMove.Any())
            {
                // Move items to another menu
                foreach (var moveAction in recipesMove)
                {
                    Data.DTO.Menus dbTarget = null;
                    if (moveAction.TargetMenu.HasValue)
                    {
                        dbTarget = this.store.Menus
                            .Where(p => p.UserId == this.Identity.UserId)
                            .SingleOrDefault(p => p.MenuId == moveAction.TargetMenu);

                        if (dbTarget == null)
                        {
                            throw new MenuNotFoundException(moveAction.TargetMenu.Value);
                        }
                    }

                    var recipeToMove =
                        moveAction.MoveAll
                        ? dbFavorites
                        : dbFavorites.Where(r => moveAction.RecipesToMove.Contains(r.RecipeId));

                    recipeToMove.ForEach(a => a.MenuId = dbTarget != null ? (Guid?)dbTarget.MenuId : null);
                }
            }

            return result;
        }

        /// <summary>
        /// Created a new menu owned by the current user.
        /// </summary>
        /// <param name="menu">A menu to create.</param>
        /// <param name="recipeIds">Zero or more recipes to add to the newly created menu.</param>
        /// <returns>A result indicating the new menu ID.</returns>
        public MenuResult CreateMenu(Menu menu, params Guid[] recipeIds)
        {
            menu.Title = menu.Title.Trim();
            var result = new MenuResult();

            Data.DTO.Menus dbMenu;
            bool dupes = this.store.Menus
                .Where(p => p.UserId == this.Identity.UserId)
                .Any(p => p.Title == menu.Title);

            if (dupes)
            {
                throw new MenuAlreadyExistsException();
            }

            this.store.Menus.Add(dbMenu = new Data.DTO.Menus
            {
                MenuId = Guid.NewGuid(),
                UserId = this.Identity.UserId,
                Title = menu.Title,
                CreatedDate = DateTime.Now,
            });

            foreach (var recipeId in recipeIds.NeverNull())
            {
                var favorite = new Favorites
                {
                    FavoriteId = Guid.NewGuid(),
                    UserId = this.Identity.UserId,
                    RecipeId = recipeId,
                    MenuId = dbMenu.MenuId
                };

                this.store.Favorites.Add(favorite);
            }

            result.MenuCreated = true;
            result.NewMenuId = dbMenu.MenuId;

            return result;
        }

        /// <summary>
        /// Imports data from another source into this context.
        /// </summary>
        /// <param name="source">Another KitchenPC context that provides the ability to export data.</param>
        public void Import(IProvisionSource source)
        {
            KPCContext.Log.DebugFormat("Importing data from {0} into StaticContext.", source.GetType().Name);
            this.InitializeStore();

            // Call source.ExportStore and populate local data store
            var data = source.ExportStore();
            var serializer = new XmlSerializer(data.GetType());

            string file = this.CompressedStore ? "KPCData.gz" : "KPCData.xml";
            string path = Path.Combine(this.DataDirectory, file);

            KPCContext.Log.DebugFormat("Serializing data to local file: {0}", path);
            using (var fileWriter = new FileStream(path, FileMode.Create))
            {
                if (this.CompressedStore)
                {
                    using (var writer = new GZipStream(fileWriter, CompressionLevel.Optimal))
                    {
                        serializer.Serialize(writer, data);
                    }
                }
                else
                {
                    serializer.Serialize(fileWriter, data);
                }
            }

            KPCContext.Log.DebugFormat("Done importing data from into StaticContext.");
        }

        /// <summary>
        /// Creates the configured data directory, if it does not already exist.
        /// </summary>
        public void InitializeStore()
        {
            if (string.IsNullOrWhiteSpace(this.DataDirectory))
            {
                throw new InvalidConfigurationException("StaticContext requires a configured data directory.");
            }

            if (!Directory.Exists(this.DataDirectory))
            {
                // Create directory
                KPCContext.Log.DebugFormat("Creating directory for local data store at: {0}", this.DataDirectory);
                Directory.CreateDirectory(this.DataDirectory);
            }
        }

        /// <summary>
        /// Exports data from this context.  This is usually called automatically by the Import method of another context.
        /// </summary>
        /// <returns>A DataStore containing all data available to this context.</returns>
        public DataStore ExportStore()
        {
            return this.store;
        }

        protected virtual void LoadTemplates()
        {
            Parser.LoadTemplates(

                // Allow partial ingredient parsing (such as "eggs")
                new Template("[ING]")
                {
                    AllowPartial = true
                },

                "[ING]: [AMT] [UNIT]", // cheddar cheese: 5 cups
                "[ING]: [AMT]", // eggs: 5
                "[FORM] [ING]: [AMT]", // shredded cheddar cheese: 1 cup
                "[FORM] [ING]: [AMT] [UNIT]", // shredded cheddar cheese: 1 cup

                "[AMT] [UNIT] [FORM] [ING]", // 5 cups melted cheddar cheese
                "[AMT] [UNIT] [ING]", // 5 cups cheddar cheese
                "[AMT] [UNIT] of [ING]", // 5 cups of cheddar cheese
                "[AMT] [UNIT] of [FORM] [ING]", // two cups of shredded cheddar cheese
                "[AMT] [ING]", // 5 eggs

                "[AMT] [UNIT] [FORM], [ING]", // 2 cups chopped, unsalted peanuts
                // "[AMT] [UNIT] [ING], [FORM]",   // 5 cups flour, sifted
                // "[AMT] [UNIT] [ING] [FORM]",    // 1 cup graham cracker crumbs
                // "[AMT] [ING] [FORM]",           // 5 graham cracker squares
                // "[AMT] [ING], [FORM]",          // 3 bananas, sliced
                "[AMT] [FORM] [ING]", // 3 mashed bananas

                // "[AMT] [ING] ([FORM])",         // 4 bananas (mashed)
                // "[AMT] [UNIT] [ING] ([FORM])",  // 5 cups flour (sifted)

                // ----- Prep notes
                "[AMT] [ING], [PREP]", // 1 carrot, shredded
                "[AMT] [UNIT] [ING], [PREP]", // 1 cup butter, melted
                "[AMT] [UNIT] [FORM] [ING], [PREP]", // 1 cup chopped walnuts, toasted
                "[AMT] [ING] - [PREP]", // 1 carrot - shredded
                "[AMT] [UNIT] [ING] - [PREP]", // 1 cup butter - melted

                // 1 cup brown sugar (optional)
                new Template("[AMT] [UNIT] [ING] (optional)")
                {
                    DefaultPrep = "optional"
                },

                // 1 cup chopped walnuts (optional)
                new Template("[AMT] [UNIT] [FORM] [ING] (optional)")
                {
                    DefaultPrep = "optional"
                },

                // new Template("[AMT] [UNIT] [ING], [FORM] (optional)") { DefaultPrep = "optional" }, // 1 cup walnuts, chopped (optional)

                // 1 cup brown sugar (divided)
                new Template("[AMT] [UNIT] [ING] (divided)")
                {
                    DefaultPrep = "divided"
                },

                // 1 cup brown sugar, divided
                new Template("[AMT] [UNIT] [ING], divided")
                {
                    DefaultPrep = "divided"
                },

                // ----- anomalies
                "[AMT] [UNIT] [ANOMALY]", // 1 cup graham cracker crumbs
                "[AMT] [UNIT] [ANOMALY], [PREP]", // 1 cup graham cracker crumbs, divided
                "[AMT] [UNIT] [ANOMALY] - [PREP]", // 1 cup graham cracker crumbs - optional
                "[AMT] [ANOMALY]", // 1 chocolate bar square
                "[AMT] [ANOMALY], [PREP]", // 1 chocolate bar square, melted
                "[AMT] [ANOMALY] - [PREP]"); // 1 chocolate bar square - melted
        }

        private ShoppingListResult UpdateShoppingList(
            Guid? listId,
            Guid[] toRemove,
            IEnumerable<ShoppingListModification> toModify,
            IEnumerable<IShoppingListSource> toAdd,
            string newName = null)
        {
            // Deletes
            if (toRemove.Any())
            {
                var dbDeletes = this.store.ShoppingListItems
                    .Where(p => p.UserId == this.Identity.UserId)
                    .Where(p => p.ShoppingListId == listId)
                    .Where(p => toRemove.Contains(p.ItemId));

                this.store.ShoppingListItems.RemoveAll(dbDeletes.Contains);
            }

            // Updates
            Guid? shoppingListId = null;
            Data.DTO.ShoppingLists dbList = null;
            List<ShoppingListItems> dbItems = null;
            if (listId.HasValue)
            {
                dbList = this.store.ShoppingLists
                   .Where(p => p.UserId == this.Identity.UserId)
                   .SingleOrDefault(p => p.ShoppingListId == listId);

                if (dbList == null)
                {
                    throw new ShoppingListNotFoundException();
                }

                dbItems = this.store.ShoppingListItems
                   .Where(p => p.UserId == this.Identity.UserId)
                   .Where(p => p.ShoppingListId.Equals(dbList.ShoppingListId))
                   .ToList();

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    dbList.Title = newName;
                }

                shoppingListId = dbList.ShoppingListId;
            }
            else
            {
                dbItems = this.store.ShoppingListItems
                    .Where(p => p.UserId == this.Identity.UserId)
                    .Where(p => p.ShoppingListId == null)
                    .ToList();
            }

            toModify.ForEach(item =>
            {
                var dbItem = this.store.ShoppingListItems.SingleOrDefault(p => p.ItemId == item.ModifiedItemId);
                if (dbItem == null)
                {
                    return;
                }

                if (item.CrossOut.HasValue)
                {
                    dbItem.CrossedOut = item.CrossOut.Value;
                }

                if (item.NewAmount != null)
                {
                    dbItem.Qty = item.NewAmount.SizeHigh;
                    dbItem.Unit = item.NewAmount.Unit;
                }
            });

            toAdd.ForEach(item =>
            {
                var source = item.GetItem();

                // Raw shopping list item
                if (source.Ingredient == null && !string.IsNullOrWhiteSpace(source.Raw))
                {
                    // Add it
                    if (!dbItems.Any(i => source.Raw.Equals(i.Raw, StringComparison.OrdinalIgnoreCase)))
                    {
                        this.store.ShoppingListItems.Add(new ShoppingListItems
                        {
                            ItemId = Guid.NewGuid(),
                            ShoppingListId = shoppingListId,
                            UserId = this.Identity.UserId,
                            Raw = source.Raw
                        });
                    }

                    return;
                }

                // Raw ingredient without any amount
                if (source.Ingredient != null && source.Amount == null)
                {
                    var existingItem = dbItems.FirstOrDefault(i => i.IngredientId.HasValue && i.IngredientId.Value == source.Ingredient.Id);

                    // Add it
                    if (existingItem == null)
                    {
                        this.store.ShoppingListItems.Add(new ShoppingListItems
                        {
                            ItemId = Guid.NewGuid(),
                            ShoppingListId = shoppingListId,
                            UserId = this.Identity.UserId,
                            IngredientId = source.Ingredient.Id
                        });
                    }
                    else
                    {
                        // Clear out existing amount
                        existingItem.Qty = null;
                        existingItem.Unit = null;
                    }
                }

                // Ingredient with amount, aggregate if necessary
                if (source.Ingredient != null && source.Amount != null)
                {
                    var existingItem = dbItems.FirstOrDefault(i => i.IngredientId.HasValue && i.IngredientId.Value == source.Ingredient.Id);

                    // Add it
                    if (existingItem == null)
                    {
                        this.store.ShoppingListItems.Add(new ShoppingListItems
                        {
                            ItemId = Guid.NewGuid(),
                            ShoppingListId = shoppingListId,
                            UserId = this.Identity.UserId,
                            IngredientId = source.Ingredient.Id,
                            Qty = source.Amount != null ? (float?)source.Amount.SizeHigh : null,
                            Unit = source.Amount != null ? (Units?)source.Amount.Unit : null
                        });
                    }
                    else if (existingItem.Qty.HasValue) // Add to total
                    {
                        existingItem.Qty += source.Amount.SizeHigh;
                    }
                }
            });

            // Load full list to return
            var indexedIngredients = this.store.GetIndexedIngredients();
            var indexedRecipes = this.store.GetIndexedRecipes();

            var items = this.store.ShoppingListItems
               .Where(p => p.UserId == this.Identity.UserId)
               .Where(l => l.ShoppingListId == shoppingListId).Select(item =>
                    new ShoppingListItem(item.ItemId)
                    {
                        Raw = item.Raw,
                        Ingredient =
                            item.IngredientId.HasValue
                            ? Data.DTO.Ingredients.ToIngredient(indexedIngredients[item.IngredientId.Value])
                            : null,
                        Recipe =
                            item.RecipeId.HasValue
                            ? Data.DTO.Recipes.ToRecipeBrief(indexedRecipes[item.RecipeId.Value])
                            : null,
                        CrossedOut = item.CrossedOut,
                        Amount =
                            item.Qty.HasValue && item.Unit.HasValue
                            ? new Amount(item.Qty.Value, item.Unit.Value)
                            : null
                    });

            return new ShoppingListResult
            {
                List = new ShoppingList(shoppingListId, dbList != null ? dbList.Title : string.Empty, items)
            };
        }
    }
}