namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class RecipeRatingsMap : ClassMap<RecipeRatings>
    {
        public RecipeRatingsMap()
        {
            this.Id(x => x.RatingId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.UserId).Not.Nullable().Index("IDX_RecipeRatings_UserId").UniqueKey("UserRating");
            this.Map(x => x.Rating).Not.Nullable();

            this.References(x => x.Recipe).Not.Nullable().Index("IDX_RecipeRatings_RecipeId").UniqueKey("UserRating");
        }
    }
}