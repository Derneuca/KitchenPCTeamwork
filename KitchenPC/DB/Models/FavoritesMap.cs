namespace KitchenPC.DB.Models
{
    using System;

    using FluentNHibernate.Mapping;

    public class FavoritesMap : ClassMap<Favorites>
    {
        public FavoritesMap()
        {
            this.Id(x => x.FavoriteId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.UserId).Not.Nullable().Index("IDX_Favorites_UserId");

            this.References(x => x.Recipe).Not.Nullable().Index("IDX_Favorites_RecipeId");
            this.References(x => x.Menu);
        }
    }
}