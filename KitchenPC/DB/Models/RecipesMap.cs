namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

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