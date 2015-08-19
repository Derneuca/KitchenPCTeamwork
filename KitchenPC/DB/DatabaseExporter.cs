namespace KitchenPC.DB.Provisioning
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Data;
    using Data.DTO;
    using log4net;
    using NHibernate;
    using NHibernate.Persister.Entity;

    public class DatabaseExporter : IDisposable, IProvisioner
    {
        private readonly IStatelessSession session;

        public DatabaseExporter(IStatelessSession session)
        {
            this.session = session;
        }

        public static ILog Log
        {
            get
            {
                return LogManager.GetLogger(typeof(DatabaseExporter));
            }
        }

        public void Dispose()
        {
            this.session.Dispose();
        }

        public List<Menus> Menus()
        {
            var list = this.ImportTableData<Models.Menus, Menus>(r => new Menus
            {
                MenuId = (Guid)r["MenuId"],
                UserId = (Guid)r["UserId"],
                Title = r["Title"] as string,
                CreatedDate = (DateTime)r["CreatedDate"]
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from Menus.", list.Count());
            return list;
        }

        public List<Recipes> Recipes()
        {
            var list = this.ImportTableData<Models.Recipes, Recipes>(r => new Recipes
            {
                RecipeId = (Guid)r["RecipeId"],
                CookTime = r["CookTime"] as short?,
                Steps = r["Steps"] as string,
                PrepTime = r["PrepTime"] as short?,
                Rating = (short)r["Rating"],
                Description = r["Description"] as string,
                Title = r["Title"] as string,
                Hidden = (bool)r["Hidden"],
                Credit = r["Credit"] as string,
                CreditUrl = r["CreditUrl"] as string,
                DateEntered = (DateTime)r["DateEntered"],
                ServingSize = (short)r["ServingSize"],
                ImageUrl = r["ImageUrl"] as string
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from Recipes.", list.Count());
            return list;
        }

        public Ingredients[] Ingredients()
        {
            var list = this.ImportTableData<Models.Ingredients, Data.DTO.Ingredients>(r => new Data.DTO.Ingredients
            {
                IngredientId = (Guid)r["IngredientId"],
                UsdaId = r["UsdaId"] as string,
                FoodGroup = r["FoodGroup"] as string,
                DisplayName = r["DisplayName"] as string,
                ManufacturerName = r["ManufacturerName"] as string,
                ConversionType = Unit.Parse<UnitType>(r["ConversionType"]),
                UnitName = r["UnitName"] as string,
                UsdaDesc = r["UsdaDesc"] as string,
                UnitWeight = (short)r["UnitWeight"]
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from Ingredients.", list.Count());
            return list;
        }

        public List<Favorites> Favorites()
        {
            var list = this.ImportTableData<Models.Favorites, Favorites>(r => new Favorites
            {
                FavoriteId = (Guid)r["FavoriteId"],
                UserId = (Guid)r["UserId"],
                RecipeId = (Guid)r["RecipeId"],
                MenuId = r["MenuId"] as Guid?
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from Favorites.", list.Count());
            return list;
        }

        public NlpPrepNotes[] NlpPrepNotes()
        {
            var list = this.ImportTableData<Models.NlpPrepNotes, NlpPrepNotes>(r => new NlpPrepNotes
            {
                Name = r["Name"] as string
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpPrepNotes.", list.Count());
            return list;
        }

        public IngredientForms[] IngredientForms()
        {
            var list = this.ImportTableData<Models.IngredientForms, IngredientForms>(r => new IngredientForms
            {
                IngredientFormId = (Guid)r["IngredientFormId"],
                IngredientId = (Guid)r["IngredientId"],
                ConversionMultiplier = (short)r["ConvMultiplier"],
                FormAmount = (float)r["FormAmount"],
                UnitType = Unit.Parse<Units>(r["UnitType"]),
                UnitName = r["UnitName"] as string,
                FormUnit = Unit.Parse<Units>(r["FormUnit"]),
                FormDisplayName = r["FormDisplayName"] as string
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from IngredientForms.", list.Count());
            return list;
        }

        public NlpFormSynonyms[] NlpFormSynonyms()
        {
            var list = this.ImportTableData<Models.NlpFormSynonyms, NlpFormSynonyms>(r => new NlpFormSynonyms
            {
                FormSynonymId = (Guid)r["FormSynonymId"],
                IngredientId = (Guid)r["IngredientId"],
                FormId = (Guid)r["FormId"],
                Name = r["Name"] as string
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpFormSynonyms.", list.Count());
            return list;
        }

        public NlpUnitSynonyms[] NlpUnitSynonyms()
        {
            var list = this.ImportTableData<Models.NlpUnitSynonyms, NlpUnitSynonyms>(r => new NlpUnitSynonyms
            {
                UnitSynonymId = (Guid)r["UnitSynonymId"],
                IngredientId = (Guid)r["IngredientId"],
                FormId = (Guid)r["FormId"],
                Name = r["Name"] as string
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpUnitSynonyms.", list.Count());
            return list;
        }

        public List<QueuedRecipes> QueuedRecipes()
        {
            var list = this.ImportTableData<Models.QueuedRecipes, QueuedRecipes>(r => new QueuedRecipes
            {
                QueueId = (Guid)r["QueueId"],
                UserId = (Guid)r["UserId"],
                RecipeId = (Guid)r["RecipeId"],
                QueuedDate = (DateTime)r["QueuedDate"]
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from QueuedRecipes.", list.Count());
            return list;
        }

        public List<RecipeRatings> RecipeRatings()
        {
            var list = this.ImportTableData<Models.RecipeRatings, RecipeRatings>(r => new RecipeRatings
            {
                RatingId = (Guid)r["RatingId"],
                UserId = (Guid)r["UserId"],
                RecipeId = (Guid)r["RecipeId"],
                Rating = (short)r["Rating"]
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from RecipeRatings.", list.Count());
            return list;
        }

        public List<ShoppingLists> ShoppingLists()
        {
            var list = this.ImportTableData<Models.ShoppingLists, ShoppingLists>(r => new ShoppingLists
            {
                ShoppingListId = (Guid)r["ShoppingListId"],
                UserId = (Guid)r["UserId"],
                Title = r["Title"] as string
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from ShoppingLists.", list.Count());
            return list;
        }

        public List<RecipeMetadata> RecipeMetadata()
        {
            var list = this.ImportTableData<Models.RecipeMetadata, RecipeMetadata>(r => new RecipeMetadata
            {
                RecipeMetadataId = (Guid)r["RecipeMetadataId"],
                RecipeId = (Guid)r["RecipeId"],
                PhotoResolution = (int)r["PhotoRes"],
                Commonality = (long)r["Commonality"],
                UsdaMatch = (bool)r["UsdaMatch"],
                MealBreakfast = (bool)r["MealBreakfast"],
                MealLunch = (bool)r["MealLunch"],
                MealDinner = (bool)r["MealDinner"],
                MealDessert = (bool)r["MealDessert"],
                DietNomeat = (bool)r["DietNomeat"],
                DietGlutenFree = (bool)r["DietGlutenFree"],
                DietNoRedMeat = (bool)r["DietNoRedMeat"],
                DietNoAnimals = (bool)r["DietNoAnimals"],
                DietNoPork = (bool)r["DietNoPork"],
                NutritionTotalfat = (short)r["NutritionTotalfat"],
                NutritionTotalSodium = (short)r["NutritionTotalSodium"],
                NutritionLowSodium = (bool)r["NutritionLowSodium"],
                NutritionLowSugar = (bool)r["NutritionLowSugar"],
                NutritionLowCalorie = (bool)r["NutritionLowCalorie"],
                NutritionTotalSugar = (short)r["NutritionTotalSugar"],
                NutritionTotalCalories = (short)r["NutritionTotalCalories"],
                NutritionLowFat = (bool)r["NutritionLowFat"],
                NutritionLowCarb = (bool)r["NutritionLowCarb"],
                NutritionTotalCarbs = (short)r["NutritionTotalCarbs"],
                SkillQuick = (bool)r["SkillQuick"],
                SkillEasyToMake = (bool)r["SkillEasy"],
                SkillCommonIngredients = (bool)r["SkillCommon"],
                TasteMildToSpicy = (short)r["TasteMildToSpicy"],
                TasteSavoryToSweet = (short)r["TasteSavoryToSweet"]
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from RecipeMetadata.", list.Count());
            return list;
        }

        public IngredientMetadata[] IngredientMetadata()
        {
            var list = this.ImportTableData<Models.IngredientMetadata, IngredientMetadata>(r => new IngredientMetadata
            {
                IngredientMetadataId = (Guid)r["IngredientMetadataId"],
                IngredientId = (Guid)r["IngredientId"],
                HasMeat = r["HasMeat"] as bool?,
                CarbsPerUnit = r["CarbsPerUnit"] as float?,
                HasRedMeat = r["HasRedMeat"] as bool?,
                SugarPerUnit = r["SugarPerUnit"] as float?,
                HasPork = r["HasPork"] as bool?,
                FatPerUnit = r["FatPerUnit"] as float?,
                SodiumPerUnit = r["SodiumPerUnit"] as float?,
                CaloriesPerUnit = r["CaloriesPerUnit"] as float?,
                Spicy = (short)r["Spicy"],
                Sweet = (short)r["Sweet"],
                HasGluten = r["HasGluten"] as bool?,
                HasAnimal = r["HasAnimal"] as bool?,
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from IngredientMetadata.", list.Count());
            return list;
        }

        public NlpDefaultPairings[] NlpDefaultPairings()
        {
            var list = this.ImportTableData<Models.NlpDefaultPairings, NlpDefaultPairings>(r => new NlpDefaultPairings
            {
                DefaultPairingId = (Guid)r["DefaultPairingId"],
                IngredientId = (Guid)r["IngredientId"],
                WeightFormId = r["WeightFormId"] as Guid?,
                VolumeFormId = r["VolumeFormId"] as Guid?,
                UnitFormId = r["UnitFormId"] as Guid?
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpDefaultPairings.", list.Count());
            return list;
        }

        public List<RecipeIngredients> RecipeIngredients()
        {
            var list = this.ImportTableData<Models.RecipeIngredients, RecipeIngredients>(r => new RecipeIngredients
            {
                RecipeIngredientId = (Guid)r["RecipeIngredientId"],
                RecipeId = (Guid)r["RecipeId"],
                IngredientId = (Guid)r["IngredientId"],
                IngredientFormId = r["IngredientFormId"] as Guid?,
                Unit = Unit.Parse<Units>(r["Unit"]),
                QtyLow = r["QtyLow"] as float?,
                DisplayOrder = (short)r["DisplayOrder"],
                PrepNote = r["PrepNote"] as string,
                Qty = r["Qty"] as float?,
                Section = r["Section"] as string
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from RecipeIngredients.", list.Count());
            return list;
        }

        public List<ShoppingListItems> ShoppingListItems()
        {
            var list = this.ImportTableData<Models.ShoppingListItems, ShoppingListItems>(r => new ShoppingListItems
            {
                ItemId = (Guid)r["ItemId"],
                Raw = r["Raw"] as string,
                Qty = r["Qty"] as float?,
                Unit = Unit.ParseNullable<Units>(r["Unit"]),
                UserId = (Guid)r["UserId"],
                IngredientId = r["IngredientId"] as Guid?,
                RecipeId = r["RecipeId"] as Guid?,
                ShoppingListId = r["ShoppingListId"] as Guid?,
                CrossedOut = (bool)r["CrossedOut"]
            }).ToList();

            Log.DebugFormat("Read {0} row(s) from ShoppingListItems.", list.Count());
            return list;
        }

        public NlpIngredientSynonyms[] NlpIngredientSynonyms()
        {
            var list = this.ImportTableData<Models.NlpIngredientSynonyms, NlpIngredientSynonyms>(r => new NlpIngredientSynonyms
            {
                IngredientSynonymId = (Guid)r["IngredientSynonymId"],
                IngredientId = (Guid)r["IngredientId"],
                Alias = r["Alias"] as string,
                PreparationNote = r["Prepnote"] as string
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpIngredientSynonyms.", list.Count());
            return list;
        }

        public NlpAnomalousIngredients[] NlpAnomalousIngredients()
        {
            var list = this.ImportTableData<Models.NlpAnomalousIngredients, NlpAnomalousIngredients>(r => new NlpAnomalousIngredients
            {
                AnomalousIngredientId = (Guid)r["AnomalousIngredientId"],
                Name = r["Name"] as string,
                IngredientId = (Guid)r["IngredientId"],
                WeightFormId = r["WeightFormId"] as Guid?,
                VolumeFormId = r["VolumeFormId"] as Guid?,
                UnitFormId = r["UnitFormId"] as Guid?
            }).ToArray();

            Log.DebugFormat("Read {0} row(s) from NlpAnomalousIngredients.", list.Count());
            return list;
        }

        private IEnumerable<D> ImportTableData<T, D>(Func<IDataReader, D> action) where T : new()
        {
            using (var cmd = this.session.Connection.CreateCommand())
            {
                var persister = this.session.GetSessionImplementation().GetEntityPersister(null, new T()) as ILockable;
                if (persister == null)
                {
                    throw new NullReferenceException();
                }

                cmd.CommandType = CommandType.TableDirect;
                cmd.CommandText = persister.RootTableName;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return action(reader);
                    }
                }
            }
        }
    }
}