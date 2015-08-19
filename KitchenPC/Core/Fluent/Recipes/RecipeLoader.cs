namespace KitchenPC.Fluent.Recipes
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to load one or more recipes.</summary>
    public class RecipeLoader
    {
        private readonly IKPCContext context;
        private readonly IList<Recipe> recipesToLoad;

        private bool withCommentCount;
        private bool withUserRating;
        private bool withCookbookStatus;
        private bool withMethod;
        private bool withPermalink;

        public RecipeLoader(IKPCContext context, Recipe recipe)
        {
            this.context = context;
            this.recipesToLoad = new List<Recipe>() { recipe };
        }

        public RecipeLoader WithCommentCount
        {
            get
            {
                this.withCommentCount = true;
                return this;
            }
        }

        public RecipeLoader WithUserRating
        {
            get
            {
                this.withUserRating = true;
                return this;
            }
        }

        public RecipeLoader WithCookbookStatus
        {
            get
            {
                this.withCookbookStatus = true;
                return this;
            }
        }

        public RecipeLoader WithMethod
        {
            get
            {
                this.withMethod = true;
                return this;
            }
        }

        public RecipeLoader WithPermalink
        {
            get
            {
                this.withPermalink = true;
                return this;
            }
        }

        public RecipeLoader Load(Recipe recipe)
        {
            this.recipesToLoad.Add(recipe);
            return this;
        }

        public IList<Recipe> List()
        {
            var options = new ReadRecipeOptions
            {
                ReturnCommentCount = this.withCommentCount,
                ReturnCookbookStatus = this.withCookbookStatus,
                ReturnMethod = this.withMethod,
                ReturnPermalink = this.withPermalink,
                ReturnUserRating = this.withUserRating
            };

            var result = this.context.ReadRecipes(this.recipesToLoad.Select(r => r.Id).ToArray(), options);
            return result;
        }
    }
}
