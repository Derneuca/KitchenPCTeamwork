namespace KitchenPC.DB.Provisioning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data.DTO;
    using log4net;
    using NHibernate;

    public class DatabaseImporter : IDisposable
    {
        private readonly ISession session;

        public DatabaseImporter(ISession session)
        {
            this.session = session;
        }

        public static ILog Log
        {
            get
            {
                return LogManager.GetLogger(typeof(DatabaseImporter));
            }
        }

        public void Dispose()
        {
            this.session.Dispose();
        }

        public void Import(IEnumerable<Menus> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var menus = new Models.Menus
                    {
                        MenuId = row.MenuId,
                        UserId = row.UserId,
                        Title = row.Title,
                        CreatedDate = row.CreatedDate
                    };

                    this.session.Save(menus, row.MenuId);
                }

                Log.DebugFormat("Created {0} row(s) in Menus", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<Recipes> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var recipes = new Models.Recipes
                    {
                        RecipeId = row.RecipeId,
                        CookTime = row.CookTime,
                        Steps = row.Steps,
                        PrepTime = row.PrepTime,
                        Rating = row.Rating,
                        Description = row.Description,
                        Title = row.Title,
                        Hidden = row.Hidden,
                        Credit = row.Credit,
                        CreditUrl = row.CreditUrl,
                        DateEntered = row.DateEntered,
                        ServingSize = row.ServingSize,
                        ImageUrl = row.ImageUrl,
                        Ingredients = new List<Models.RecipeIngredients>()
                    };

                    this.session.Save(recipes, row.RecipeId);
                }

                Log.DebugFormat("Created {0} row(s) in Recipes", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<Favorites> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var favorites = new Models.Favorites
                    {
                        FavoriteId = row.FavoriteId,
                        UserId = row.UserId,
                        Recipe = Models.Recipes.FromId(row.RecipeId),
                        Menu = row.MenuId.HasValue ? Models.Menus.FromId(row.MenuId.Value) : null
                    };

                    this.session.Save(favorites, row.FavoriteId);
                }

                Log.DebugFormat("Created {0} row(s) in Favorites", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<Ingredients> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var ingredients = new Models.Ingredients
                    {
                        IngredientId = row.IngredientId,
                        FoodGroup = row.FoodGroup,
                        UsdaId = row.UsdaId,
                        UnitName = row.UnitName,
                        ManufacturerName = row.ManufacturerName,
                        ConversionType = row.ConversionType,
                        UnitWeight = row.UnitWeight,
                        DisplayName = row.DisplayName,
                        UsdaDesc = row.UsdaDesc
                    };

                    this.session.Save(ingredients, row.IngredientId);
                }

                Log.DebugFormat("Created {0} row(s) in Ingredients", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpPrepNotes> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var notes = new Models.NlpPrepNotes
                    {
                        Name = row.Name
                    };

                    this.session.Save(notes);
                }

                Log.DebugFormat("Created {0} row(s) in NlpPrepNotes", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<QueuedRecipes> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var queuedRecipes = new Models.QueuedRecipes
                    {
                        QueueId = row.QueueId,
                        UserId = row.UserId,
                        Recipe = Models.Recipes.FromId(row.RecipeId),
                        QueuedDate = row.QueuedDate
                    };

                    this.session.Save(queuedRecipes, row.QueueId);
                }

                Log.DebugFormat("Created {0} row(s) in QueuedRecipes", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<RecipeRatings> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var recipeRatings = new Models.RecipeRatings
                    {
                        RatingId = row.RatingId,
                        UserId = row.UserId,
                        Recipe = Models.Recipes.FromId(row.RecipeId),
                        Rating = row.Rating
                    };

                    this.session.Save(recipeRatings, row.RatingId);
                }

                Log.DebugFormat("Created {0} row(s) in RecipeRatings", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<ShoppingLists> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var shoppingLists = new Models.ShoppingLists
                    {
                        ShoppingListId = row.ShoppingListId,
                        UserId = row.UserId,
                        Title = row.Title
                    };

                    this.session.Save(shoppingLists, row.ShoppingListId);
                }

                Log.DebugFormat("Created {0} row(s) in ShoppingLists", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<RecipeMetadata> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var recipeMetadata = new Models.RecipeMetadata
                    {
                        RecipeMetadataId = row.RecipeMetadataId,
                        Recipe = Models.Recipes.FromId(row.RecipeId),
                        PhotoRes = row.PhotoResolution,
                        Commonality = row.Commonality,
                        UsdaMatch = row.UsdaMatch,
                        MealBreakfast = row.MealBreakfast,
                        MealLunch = row.MealLunch,
                        MealDinner = row.MealDinner,
                        MealDessert = row.MealDessert,
                        DietNomeat = row.DietNomeat,
                        DietGlutenFree = row.DietGlutenFree,
                        DietNoRedMeat = row.DietNoRedMeat,
                        DietNoAnimals = row.DietNoAnimals,
                        DietNoPork = row.DietNoPork,
                        NutritionTotalfat = row.NutritionTotalfat,
                        NutritionTotalSodium = row.NutritionTotalSodium,
                        NutritionLowSodium = row.NutritionLowSodium,
                        NutritionLowSugar = row.NutritionLowSugar,
                        NutritionLowCalorie = row.NutritionLowCalorie,
                        NutritionTotalSugar = row.NutritionTotalSugar,
                        NutritionTotalCalories = row.NutritionTotalCalories,
                        NutritionLowFat = row.NutritionLowFat,
                        NutritionLowCarb = row.NutritionLowCarb,
                        NutritionTotalCarbs = row.NutritionTotalCarbs,
                        SkillQuick = row.SkillQuick,
                        SkillEasy = row.SkillEasyToMake,
                        SkillCommon = row.SkillCommonIngredients,
                        TasteMildToSpicy = row.TasteMildToSpicy,
                        TasteSavoryToSweet = row.TasteSavoryToSweet
                    };

                    this.session.Save(recipeMetadata, row.RecipeMetadataId);
                }

                Log.DebugFormat("Created {0} row(s) in RecipeMetadata", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpFormSynonyms> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var formSynonyms = new Models.NlpFormSynonyms
                    {
                        FormSynonymId = row.FormSynonymId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        Form = Models.IngredientForms.FromId(row.FormId),
                        Name = row.Name
                    };

                    this.session.Save(formSynonyms, row.FormSynonymId);
                }

                Log.DebugFormat("Created {0} row(s) in NlpFormSynonyms", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<IngredientForms> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var ingredientForms = new Models.IngredientForms
                    {
                        IngredientFormId = row.IngredientFormId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        ConvMultiplier = row.ConversionMultiplier,
                        FormAmount = row.FormAmount,
                        UnitType = row.UnitType,
                        UnitName = row.UnitName,
                        FormUnit = row.FormUnit,
                        FormDisplayName = row.FormDisplayName
                    };

                    this.session.Save(ingredientForms, row.IngredientFormId);
                }

                Log.DebugFormat("Created {0} row(s) in IngredientForms", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpUnitSynonyms> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var unitSynonyms = new Models.NlpUnitSynonyms
                    {
                        UnitSynonymId = row.UnitSynonymId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        Form = Models.IngredientForms.FromId(row.FormId),
                        Name = row.Name
                    };

                    this.session.Save(unitSynonyms, row.UnitSynonymId);
                }

                Log.DebugFormat("Created {0} row(s) in NlpUnitSynonyms", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<RecipeIngredients> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var recipeIngredients = new Models.RecipeIngredients
                    {
                        RecipeIngredientId = row.RecipeIngredientId,
                        Recipe = Models.Recipes.FromId(row.RecipeId),
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        IngredientForm = row.IngredientFormId.HasValue ? Models.IngredientForms.FromId(row.IngredientFormId.Value) : null,
                        Unit = row.Unit,
                        QtyLow = row.QtyLow,
                        DisplayOrder = row.DisplayOrder,
                        PrepNote = row.PrepNote,
                        Qty = row.Qty,
                        Section = row.Section
                    };

                    this.session.Save(recipeIngredients, row.RecipeIngredientId);
                }

                Log.DebugFormat("Created {0} row(s) in RecipeIngredients", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<ShoppingListItems> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var shoppingListItems = new Models.ShoppingListItems
                    {
                        ItemId = row.ItemId,
                        Raw = row.Raw,
                        Qty = row.Qty,
                        Unit = row.Unit,
                        UserId = row.UserId,
                        Ingredient = row.IngredientId.HasValue ? Models.Ingredients.FromId(row.IngredientId.Value) : null,
                        Recipe = row.RecipeId.HasValue ? Models.Recipes.FromId(row.RecipeId.Value) : null,
                        ShoppingList = row.ShoppingListId.HasValue ? Models.ShoppingLists.FromId(row.ShoppingListId.Value) : null,
                        CrossedOut = row.CrossedOut
                    };

                    this.session.Save(shoppingListItems, row.ItemId);
                }

                Log.DebugFormat("Created {0} row(s) in ShoppingListItems", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpDefaultPairings> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var defaultPairings = new Models.NlpDefaultPairings
                    {
                        DefaultPairingId = row.DefaultPairingId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        WeightForm = row.WeightFormId.HasValue ? Models.IngredientForms.FromId(row.WeightFormId.Value) : null,
                        VolumeForm = row.VolumeFormId.HasValue ? Models.IngredientForms.FromId(row.VolumeFormId.Value) : null,
                        UnitForm = row.UnitFormId.HasValue ? Models.IngredientForms.FromId(row.UnitFormId.Value) : null
                    };

                    this.session.Save(defaultPairings, row.DefaultPairingId);
                }

                Log.DebugFormat("Created {0} row(s) in NlpDefaultPairings", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<IngredientMetadata> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var ingredientMetadata = new Models.IngredientMetadata
                    {
                        IngredientMetadataId = row.IngredientMetadataId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        HasMeat = row.HasMeat,
                        CarbsPerUnit = row.CarbsPerUnit,
                        HasRedMeat = row.HasRedMeat,
                        SugarPerUnit = row.SugarPerUnit,
                        HasPork = row.HasPork,
                        FatPerUnit = row.FatPerUnit,
                        SodiumPerUnit = row.SodiumPerUnit,
                        CaloriesPerUnit = row.CaloriesPerUnit,
                        Spicy = row.Spicy,
                        Sweet = row.Sweet,
                        HasGluten = row.HasGluten,
                        HasAnimal = row.HasAnimal
                    };

                    this.session.Save(ingredientMetadata, row.IngredientMetadataId);
                }

                Log.DebugFormat("Created {0} row(s) in IngredientMetadata", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpIngredientSynonyms> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var ingredientSynonyms = new Models.NlpIngredientSynonyms
                    {
                        IngredientSynonymId = row.IngredientSynonymId,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        Alias = row.Alias,
                        Prepnote = row.PreparationNote
                    };

                    this.session.Save(ingredientSynonyms, row.IngredientSynonymId);
                }

                Log.DebugFormat("Created {0} row(s) in NlpIngredientSynonyms", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }

        public void Import(IEnumerable<NlpAnomalousIngredients> data)
        {
            using (var transaction = this.session.BeginTransaction())
            {
                var d = data.ToArray();
                foreach (var row in d)
                {
                    var anomalousIngredients = new Models.NlpAnomalousIngredients
                    {
                        AnomalousIngredientId = row.AnomalousIngredientId,
                        Name = row.Name,
                        Ingredient = Models.Ingredients.FromId(row.IngredientId),
                        WeightForm = row.WeightFormId.HasValue ? Models.IngredientForms.FromId(row.WeightFormId.Value) : null,
                        VolumeForm = row.VolumeFormId.HasValue ? Models.IngredientForms.FromId(row.VolumeFormId.Value) : null,
                        UnitForm = row.UnitFormId.HasValue ? Models.IngredientForms.FromId(row.UnitFormId.Value) : null
                    };

                    this.session.Save(anomalousIngredients, row.AnomalousIngredientId);
                }

                Log.DebugFormat("Created {0} row(s) in NlpAnomalousIngredients", d.Count());
                transaction.Commit();
                this.session.Flush();
            }
        }
    }
}