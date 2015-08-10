namespace KitchenPC.Recipes
{
    using System;

    public class RecipeBrief
    {
        private Uri recipeimg;

        public RecipeBrief()
        {
            this.AvgRating = 0;
        }

        public RecipeBrief(Recipe recipe)
        {
            this.Id = recipe.Id;
            this.OwnerId = recipe.OwnerId;
            this.Title = recipe.Title;
            this.Description = recipe.Description;
            this.ImageUrl = recipe.ImageUrl;
            this.Author = recipe.OwnerAlias;
            this.PrepTime = recipe.PrepTime;
            this.CookTime = recipe.CookTime;
            this.AvgRating = recipe.AvgRating;
        }

        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Permalink { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public short? PrepTime { get; set; }

        public short? CookTime { get; set; }

        public short AvgRating { get; set; }

        public string ImageUrl
        {
            get
            {
                return this.recipeimg == null ? "/Images/img_placeholder.png" : this.recipeimg.ToString();
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.recipeimg = null;
                    return;
                }

                // UriBuilder builder = new UriBuilder(baseUri);
                var builder = new UriBuilder();
                builder.Path = "Thumb_" + value;
                this.recipeimg = builder.Uri;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Title, this.Id);
        }
    }
}