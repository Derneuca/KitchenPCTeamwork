namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Recipes;

    /// <summary>
    /// Captures an instance of the database in memory for modeling without requiring DB hits.
    /// This "cache" only contains the exact information it needs and is optimized for modeling performance.
    /// </summary>
    public sealed partial class DBSnapshot
    {
        /// <summary>
        /// A proxy class to allow an IDBLoader to populate indexes within a DBSnapshot.  This is IDisposable since this object might use
        /// large amount of memory while building indexes, and a GC should be forced when object is disposed.
        /// </summary>
        private class Indexer : IDisposable
        {
            private readonly DBSnapshot snapshot;
            private RatingGraph ratingGraph;

            public Indexer(DBSnapshot snapshot)
            {
                this.snapshot = snapshot;
                this.snapshot.recipeMap = new Dictionary<Guid, RecipeNode>();
                this.snapshot.ingredientMap = new Dictionary<Guid, IngredientNode>();
                this.snapshot.recipeList = new IEnumerable<RecipeNode>[RecipeTag.NumberOfTags];
            }

            public void Index(IKPCContext context)
            {
                var timer = new Stopwatch();
                timer.Start();

                this.ratingGraph = new RatingGraph();
                var loader = context.ModelerLoader;

                this.CreateRatingGraph(loader);

                ModelingSession.Log.InfoFormat("Building Rating Graph took {0}ms.", timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();

                this.CreateEmptyRecipeNodes(loader);

                ModelingSession.Log.InfoFormat("Building empty RecipeNodes took {0}ms.", timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();

                this.IndexRecipes();

                ModelingSession.Log.InfoFormat("Indexing recipes by tag took {0}ms.", timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();

                this.CreatIngredientUsageVertices(loader);

                ModelingSession.Log.InfoFormat("Creating IngredientUsage vertices took {0}ms.", timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();

                this.CreateSuggestionLinks();

                ModelingSession.Log.InfoFormat("Building suggestions for each recipe took {0}ms.", timer.ElapsedMilliseconds);
                timer.Reset();
            }

            public void Dispose()
            {
                var timer = new Stopwatch();
                timer.Start();

                this.ratingGraph = null;

                // Free up memory/increase Index accessing speed by converting List<> objects to arrays
                foreach (var recipe in this.snapshot.recipeMap.Values)
                {
                    recipe.Ingredients = recipe.Ingredients.ToArray();
                }

                foreach (var ingredientNode in this.snapshot.ingredientMap.Values)
                {
                    var temp = new List<RecipeNode[]>();
                    int usedTags = 0;

                    for (int count = 0; count < RecipeTag.NumberOfTags; count++)
                    {
                        RecipeNode[] nodes = null;
                        if (ingredientNode.RecipesByTag[count] != null)
                        {
                            nodes = ingredientNode.RecipesByTag[count].ToArray();
                            usedTags += 1 << count;
                        }

                        temp.Add(nodes);
                    }

                    ingredientNode.RecipesByTag = temp.ToArray();
                    ingredientNode.AvailableTags = usedTags;
                }

                // Force garbage collection now, since there might be several hundred megs of unreachable allocations
                GC.Collect(); 

                timer.Stop();
                ModelingSession.Log.InfoFormat("Cleaning up Indexer took {0}ms.", timer.ElapsedMilliseconds);
            }

            private void CreateRatingGraph(IModelerLoader loader)
            {
                foreach (var dataItem in loader.LoadRatingGraph())
                {
                    var rating = dataItem.Rating;
                    var userId = dataItem.UserId;
                    var recipeId = dataItem.RecipeId;

                    // Rating too low to worry about
                    if (rating < 4) 
                    {
                        continue; // TODO: Might not be needed, DB should only query for 4 or 5 star ratings
                    }

                    this.ratingGraph.AddRating(rating, userId, recipeId);
                }
            }

            /// <summary>
            /// Creates empty recipe nodes without links
            /// </summary>
            private void CreateEmptyRecipeNodes(IModelerLoader loader)
            {
                this.snapshot.recipeMap = (from o in loader.LoadRecipeGraph()
                                           select new RecipeNode()
                                           {
                                               RecipeId = o.Id,
                                               Rating = o.Rating,
                                               Tags = o.Tags,
                                               Hidden = o.Hidden,
                                               Ingredients = new List<IngredientUsage>()
                                           }).ToDictionary(k => k.RecipeId);
            }

            private void IndexRecipes()
            {
                foreach (var recipeNode in this.snapshot.recipeMap.Values)
                {
                    if (recipeNode.Hidden)
                    {
                        // recipeList does not include Hidden recipes so they don't get picked at random
                        continue;
                    }

                    foreach (var tag in recipeNode.Tags)
                    {
                        var nodes = this.snapshot.recipeList[tag.Value] as List<RecipeNode>;
                        if (nodes == null)
                        {
                            this.snapshot.recipeList[tag.Value] = nodes = new List<RecipeNode>();
                        }

                        nodes.Add(recipeNode);
                    }
                }

                for (var i = 0; i < this.snapshot.recipeList.Length; i++)
                {
                    var list = this.snapshot.recipeList[i] as List<RecipeNode>;
                    if (list != null)
                    {
                        this.snapshot.recipeList[i] = list.ToArray();
                    }
                    else 
                    {
                        // No recipes in DB use this tag
                        this.snapshot.recipeList[i] = new RecipeNode[0];
                    }
                }
            }

            private void CreatIngredientUsageVertices(IModelerLoader loader)
            {
                // Loop through ingredient usages and fill in vertices on graph
                // For each item: Create IngredientUsage and add to recipe, create IngredientNode (if necessary) and add recipe to IngredientNode
                foreach (var o in loader.LoadIngredientGraph())
                {
                    var recipeId = o.RecipeId;
                    var ingredientId = o.IngredientId;
                    var qty = o.Qty;
                    var unit = o.Unit;
                    var convType = Unit.GetConvertionType(unit);

                    List<RecipeNode>[] nodes;
                    IngredientNode ingredientNode;
                    var node = this.snapshot.recipeMap[recipeId];

                    // New ingredient, create node for it
                    if (!this.snapshot.ingredientMap.TryGetValue(ingredientId, out ingredientNode)) 
                    {
                        nodes = new List<RecipeNode>[RecipeTag.NumberOfTags];
                        ingredientNode = new IngredientNode()
                            {
                                IngredientId = ingredientId,
                                RecipesByTag = nodes,
                                ConversionType = convType
                            };
                        this.snapshot.ingredientMap.Add(ingredientId, ingredientNode);
                    }
                    else
                    {
                        nodes = ingredientNode.RecipesByTag as List<RecipeNode>[];
                    }

                    // For each tag the recipe has, we need to create a link through ingNode.RecipesByTag to the recipe
                    // Don't Index Hidden recipes
                    if (!node.Hidden)
                    {
                        foreach (var tag in node.Tags)
                        {
                            if (nodes[tag.Value] == null)
                            {
                                nodes[tag.Value] = new List<RecipeNode>();
                            }

                            // Add ingredient link to RecipeNode
                            nodes[tag.Value].Add(node); 
                        }
                    }

                    // Add ingredient usage to recipe
                    var usages = node.Ingredients as List<IngredientUsage>;
                    usages.Add(new IngredientUsage()
                    {
                        Amount = qty,
                        Ingredient = ingredientNode,
                        Unit = unit
                    });
                }
            }

            /// <summary>
            /// Creates suggestion links for each recipe
            /// </summary>
            private void CreateSuggestionLinks()
            {
                foreach (var recipe in this.snapshot.recipeMap.Values)
                {
                    recipe.Suggestions = (from suggestion in this.ratingGraph.GetSimilarRecipes(recipe.RecipeId)
                                          select this.snapshot.recipeMap[suggestion]).ToArray();
                }
            }
        }
    }

    public sealed partial class DBSnapshot
    {
        private Dictionary<Guid, RecipeNode> recipeMap; // Recipe Index (will include hidden recipes)
        private Dictionary<Guid, IngredientNode> ingredientMap; // Ingredient Index
        private IEnumerable<RecipeNode>[] recipeList; // Ordinal recipe Index keyed by tag (for picking random recipes)

        public DBSnapshot(IKPCContext context)
        {
            var timer = new Stopwatch();
            timer.Start();

            using (var i = new Indexer(this))
            {
                i.Index(context);
            }

            timer.Stop();
            ModelingSession.Log.InfoFormat("Total time building snapshot was {0}ms.", timer.ElapsedMilliseconds);
        }

        public int RecipeCount
        {
            get
            {
                return this.recipeMap.Keys.Count;
            }
        }

        public RecipeNode FindRecipe(Guid id)
        {
            var result = this.recipeMap.ContainsKey(id) ? this.recipeMap[id] : null;
            return result;
        }

        public RecipeNode[] FindRecipesByTag(int tag)
        {
            var result = this.recipeList[tag] as RecipeNode[];
            return result;
        }

        public IngredientNode FindIngredient(Guid id)
        {
            var result = this.ingredientMap.ContainsKey(id) ? this.ingredientMap[id] : null;
            return result;
        }
    }
}