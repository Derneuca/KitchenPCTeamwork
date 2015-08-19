namespace KitchenPC.DB.Models
{
    using System;
    using FluentNHibernate.Mapping;

    public class MenusMap : ClassMap<Menus>
    {
        public MenusMap()
        {
            this.Id(x => x.MenuId)
                .GeneratedBy.GuidComb()
                .UnsavedValue(Guid.Empty);

            this.Map(x => x.UserId).Not.Nullable().Index("IDX_Menus_UserId").UniqueKey("UserTitle");
            this.Map(x => x.Title).Not.Nullable().UniqueKey("UserTitle");
            this.Map(x => x.CreatedDate).Not.Nullable();

            this.HasMany(x => x.Recipes)
                .KeyColumn("MenuId")
                .Cascade.Delete(); // If Menu is deleted, delete all the Favorites that reference this menu
        }
    }
}