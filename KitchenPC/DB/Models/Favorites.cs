namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class Favorites
    {
        public virtual Guid FavoriteId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual Recipes Recipe { get; set; }

        public virtual Menus Menu { get; set; }
    }

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