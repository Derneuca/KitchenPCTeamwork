namespace KitchenPC.DB.Search
{
    using System.Linq;
    using Context.Interfaces;
    using Context;
    using Models;
    using NHibernate.Criterion;
    using NHibernate.Criterion.Lambda;
    using Recipes;
    using Recipes.Enums;

    /// <summary>
    /// A basic, platform-agnostic search provider implemented with NHibernate.
    /// </summary>
    public class NHSearch : ISearchProvider
    {
        private readonly DatabaseAdapter adapter;

        public NHSearch(DatabaseAdapter adapter)
        {
            this.adapter = adapter;
        }

        public static NHSearch Instance(DatabaseAdapter adapter)
        {
            return new NHSearch(adapter);
        }

        public SearchResults Search(AuthIdentity identity, RecipeQuery query)
        {
            using (var session = this.adapter.GetSession())
            {
                Recipes recipe = null;

                var q = session.QueryOver(() => recipe)
                   .Where(p => !p.Hidden);

                if (!string.IsNullOrWhiteSpace(query.Keywords))
                {
                    q = q.Where(
                        Restrictions.Or(
                            Restrictions.InsensitiveLike("Title", string.Format("%{0}%", query.Keywords.Trim())),
                            Restrictions.InsensitiveLike("Description", string.Format("%{0}%", query.Keywords.Trim()))));
                }

                if (query.Time.MaxPrep.HasValue)
                {
                    q = q.Where(p => p.PrepTime <= query.Time.MaxPrep.Value);
                }

                if (query.Time.MaxCook.HasValue)
                {
                    q = q.Where(p => p.CookTime <= query.Time.MaxCook.Value);
                }

                if (query.Rating > 0)
                {
                    q = q.Where(p => p.Rating >= (int)query.Rating.Value);
                }

                if (query.Include != null && query.Include.Length > 0)
                {
                    // Create a sub-query for ingredients to include
                    q = q.WithSubquery
                       .WhereExists(QueryOver.Of<RecipeIngredients>()
                          .Where(item => item.Recipe.RecipeId == recipe.RecipeId)
                          .Where(Restrictions.InG("Ingredient", query.Include.Select(Ingredients.FromId).ToArray()))
                          .Select(i => i.RecipeIngredientId).Take(1));
                }

                if (query.Exclude != null && query.Exclude.Length > 0)
                {
                    // Create a sub-query for ingredients to exclude
                    q = q.WithSubquery
                       .WhereNotExists(QueryOver.Of<RecipeIngredients>()
                          .Where(item => item.Recipe.RecipeId == recipe.RecipeId)
                          .Where(Restrictions.InG("Ingredient", query.Exclude.Select(Ingredients.FromId).ToArray()))
                          .Select(i => i.RecipeIngredientId).Take(1));
                }

                if (query.Photos == PhotoFilter.Photo || query.Photos == PhotoFilter.HighRes)
                {
                    q = q.Where(Restrictions.IsNotNull("ImageUrl"));
                }

                if (query.Diet || query.Nutrition || query.Skill || query.Taste || (query.Meal != MealFilter.All) || (query.Photos == PhotoFilter.HighRes))
                {
                    RecipeMetadata metadata = null;
                    q = q.JoinAlias(r => r.RecipeMetadata, () => metadata);

                    if (query.Meal != MealFilter.All)
                    {
                        if (query.Meal == MealFilter.Breakfast)
                        {
                            q = q.Where(() => metadata.MealBreakfast);
                        }

                        if (query.Meal == MealFilter.Dessert)
                        {
                            q = q.Where(() => metadata.MealDessert);
                        }

                        if (query.Meal == MealFilter.Dinner)
                        {
                            q = q.Where(() => metadata.MealDinner);
                        }

                        if (query.Meal == MealFilter.Lunch)
                        {
                            q = q.Where(() => metadata.MealLunch);
                        }
                    }

                    if (query.Photos == PhotoFilter.HighRes)
                    {
                        q = q.Where(() => metadata.PhotoRes >= 1024 * 768);
                    }

                    if (query.Diet.GlutenFree)
                    {
                        q = q.Where(() => metadata.DietGlutenFree);
                    }

                    if (query.Diet.NoAnimals)
                    {
                        q = q.Where(() => metadata.DietNoAnimals);
                    }

                    if (query.Diet.NoMeat)
                    {
                        q = q.Where(() => metadata.DietNomeat);
                    }

                    if (query.Diet.NoPork)
                    {
                        q = q.Where(() => metadata.DietNoPork);
                    }

                    if (query.Diet.NoRedMeat)
                    {
                        q = q.Where(() => metadata.DietNoRedMeat);
                    }

                    if (query.Nutrition.LowCalorie)
                    {
                        q = q.Where(() => metadata.NutritionLowCalorie);
                    }

                    if (query.Nutrition.LowCarb)
                    {
                        q = q.Where(() => metadata.NutritionLowCarb);
                    }

                    if (query.Nutrition.LowFat)
                    {
                        q = q.Where(() => metadata.NutritionLowFat);
                    }

                    if (query.Nutrition.LowSodium)
                    {
                        q = q.Where(() => metadata.NutritionLowSodium);
                    }

                    if (query.Nutrition.LowSugar)
                    {
                        q = q.Where(() => metadata.NutritionLowSugar);
                    }

                    if (query.Skill.Common)
                    {
                        q = q.Where(() => metadata.SkillCommon).OrderBy(() => metadata.Commonality).Desc();
                    }

                    if (query.Skill.Easy)
                    {
                        q = q.Where(() => metadata.SkillEasy);
                    }

                    if (query.Skill.Quick)
                    {
                        q = q.Where(() => metadata.SkillQuick);
                    }

                    if (query.Taste.MildToSpicy != SpicinessLevel.Medium)
                    {
                        q = query.Taste.MildToSpicy < SpicinessLevel.Medium
                           ? q.Where(() => metadata.TasteMildToSpicy <= query.Taste.Spiciness).OrderBy(() => metadata.TasteMildToSpicy).Asc()
                           : q.Where(() => metadata.TasteMildToSpicy >= query.Taste.Spiciness).OrderBy(() => metadata.TasteMildToSpicy).Desc();
                    }

                    if (query.Taste.SavoryToSweet != SweetnessLevel.Medium)
                    {
                        q = query.Taste.SavoryToSweet < SweetnessLevel.Medium
                           ? q.Where(() => metadata.TasteSavoryToSweet <= query.Taste.Sweetness).OrderBy(() => metadata.TasteSavoryToSweet).Asc()
                           : q.Where(() => metadata.TasteSavoryToSweet >= query.Taste.Sweetness).OrderBy(() => metadata.TasteSavoryToSweet).Desc();
                    }
                }

                IQueryOverOrderBuilder<Recipes, Recipes> orderBy;

                switch (query.Sort)
                {
                    case SortOrder.Title:
                        orderBy = q.OrderBy(p => p.Title);
                        break;
                    case SortOrder.PrepTime:
                        orderBy = q.OrderBy(p => p.PrepTime);
                        break;
                    case SortOrder.CookTime:
                        orderBy = q.OrderBy(p => p.CookTime);
                        break;
                    case SortOrder.Image:
                        orderBy = q.OrderBy(p => p.ImageUrl);
                        break;
                    default:
                        orderBy = q.OrderBy(p => p.Rating);
                        break;
                }

                var results = (query.Direction == SortDirection.Descending ? orderBy.Desc() : orderBy.Asc())
                   .Skip(query.Offset)
                   .Take(100)
                   .List();

                return new SearchResults
                {
                    Briefs = results.Select(r => r.AsRecipeBrief()).ToArray(),
                    TotalCount = results.Count
                };
            }
        }
    }
}