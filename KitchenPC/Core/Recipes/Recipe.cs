namespace KitchenPC.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using KitchenPC.Ingredients;
    using KitchenPC.Recipes.Enums;

    public class Recipe
    {
        private readonly List<IngredientUsage> ingredients;

        private short? prepTime;
        private short? cookTime;
        private short servingSize;
        private string title;
        private string description;
        private string creditUrl;
        private RecipeTags tags;

        public Recipe(Guid id, string title, string description, string imageUrl)
        {
            this.ingredients = new List<IngredientUsage>();
            this.PrepTime = 0;
            this.CookTime = 0;
            this.AvgRating = 0;
            this.ServingSize = 4;
            this.Comments = 0;
            this.InMenus = 0;
            this.PublicEdit = false;
            this.AllowDelete = false;
            this.UserRating = Rating.None;
            this.Tags = null;
            this.Id = id;
            this.Title = title;
            this.Description = description;
            this.ImageUrl = imageUrl;
        }

        public Recipe()
            : this(Guid.Empty, null, null, null)
        {
        }

        #region PROPERTIES

        public IngredientUsage[] Ingredients
        {
            get
            {
                return this.ingredients.ToArray();
            }

            set
            {
                this.ingredients.Clear();
                if (value == null || value.Length == 0)
                {
                    throw new InvalidRecipeDataException("Recipes must contain at least one ingredient.");
                }

                this.ingredients.AddRange(value);
            }
        }

        public short? PrepTime
        {
            get
            {
                return this.prepTime;
            }

            set
            {
                if (value < 0)
                {
                    throw new InvalidRecipeDataException("PrepTime must be equal to or greater than zero.");
                }

                this.prepTime = value;
            }
        }

        public short? CookTime
        {
            get
            {
                return this.cookTime;
            }

            set
            {
                if (value < 0)
                {
                    throw new InvalidRecipeDataException("CookTime must be equal to or greater than zero.");
                }

                this.cookTime = value;
            }
        }

        public short ServingSize
        {
            get
            {
                return this.servingSize;
            }

            set
            {
                if (value <= 0)
                {
                    throw new InvalidRecipeDataException("ServingSize must be greater than zero.");
                }

                this.servingSize = value;
            }
        }

        public RecipeTags Tags
        {
            get
            {
                return this.tags;
            }

            set
            {
                if (value == null || value.Length == 0)
                {
                    throw new InvalidRecipeDataException("Recipes must contain at least one tag.");
                }

                this.tags = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidRecipeDataException("A recipe title is required.");
                }

                this.title = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (value != null && value.Length > 250)
                {
                    value = value.Substring(0, 250) + "...";
                }

                this.description = value;
            }
        }

        public string CreditUrl
        {
            get
            {
                return this.creditUrl;
            }

            set
            {
                if (value != null)
                {
                    const string Pattern = @"^https?\://(?<domain>[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3})(/\S*)?$";
                    var match = Regex.Match(value, Pattern);
                    if (match.Success && match.Groups["domain"].Success)
                    {
                        this.creditUrl = match.Value;
                        this.Credit = match.Groups["domain"].Value.ToLower(); // TODO: Clean up domain name
                    }
                    else 
                    {
                        // Bad URL, clear Credit info
                        this.CreditUrl = null;
                        this.Credit = null;
                    }
                }
            }
        }

        public string Credit { get; set; }

        public string OwnerAlias { get; set; }

        public string ImageUrl { get; set; }

        public string Permalink { get; set; }

        public string Method { get; set; }

        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public short AvgRating { get; set; }
        
        public int Comments { get; set; }

        public int InMenus { get; set; }

        public bool PublicEdit { get; set; }

        public bool AllowDelete { get; set; }

        public DateTime DateEntered { get; set; }

        public Rating UserRating { get; set; }

        #endregion PROPERTIES

        public static Recipe FromId(Guid recipeId)
        {
            return new Recipe(recipeId, null, null, null);
        }

        public void AddIngredient(IngredientUsage ingredient)
        {
            this.ingredients.Add(ingredient);
        }

        public void AddIngredients(IEnumerable<IngredientUsage> ingredients)
        {
            foreach (var ingredientUsage in ingredients)
            {
                this.AddIngredient(ingredientUsage);
            }
        }
    }
}