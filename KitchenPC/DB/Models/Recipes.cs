namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;
    using FluentNHibernate.Mapping;
    using KitchenPC.Recipes;

    public class Recipes
    {
        public virtual Guid RecipeId { get; set; }

        public virtual short? CookTime { get; set; }

        public virtual string Steps { get; set; }

        public virtual short? PrepTime { get; set; }

        public virtual short Rating { get; set; }

        public virtual string Description { get; set; }

        public virtual string Title { get; set; }

        public virtual bool Hidden { get; set; }

        public virtual string Credit { get; set; }

        public virtual string CreditUrl { get; set; }

        public virtual DateTime DateEntered { get; set; }

        public virtual short ServingSize { get; set; }

        public virtual string ImageUrl { get; set; }

        public virtual IList<RecipeIngredients> Ingredients { get; set; }

        public virtual RecipeMetadata RecipeMetadata { get; set; }

        public static Recipes FromId(Guid id)
        {
            return new Recipes
            {
                RecipeId = id
            };
        }

        public virtual RecipeBrief AsRecipeBrief()
        {
            return new RecipeBrief
            {
                Id = this.RecipeId,
                ImageUrl = this.ImageUrl,
                AvgRating = this.Rating,
                CookTime = this.CookTime,
                PrepTime = this.PrepTime,
                Description = this.Description,
                Title = this.Title
            };
        }
    }

    public class RecipesMap : ClassMap<Recipes>
    {
        public RecipesMap()
        {
            this.Id(x => x.RecipeId)
               .GeneratedBy.GuidComb()
               .UnsavedValue(Guid.Empty);

            this.Map(x => x.CookTime).Index("IDX_Recipes_Cooktime");
            this.Map(x => x.Steps).Length(10000);
            this.Map(x => x.PrepTime).Index("IDX_Recipes_Preptime");
            this.Map(x => x.Rating).Not.Nullable().Index("IDX_Recipes_Rating");
            this.Map(x => x.Description).Length(512);
            this.Map(x => x.Title).Not.Nullable().Length(100);
            this.Map(x => x.Hidden).Not.Nullable().Index("IDX_Recipes_Hidden");
            this.Map(x => x.Credit).Length(100);
            this.Map(x => x.CreditUrl).Length(1024);
            this.Map(x => x.DateEntered).Not.Nullable();
            this.Map(x => x.ServingSize).Not.Nullable().Check("ServingSize > 0");
            this.Map(x => x.ImageUrl).Length(100);

            this.HasMany(x => x.Ingredients).KeyColumn("RecipeId");
            this.HasOne(x => x.RecipeMetadata).PropertyRef(x => x.Recipe).Cascade.All();
        }
    }
}