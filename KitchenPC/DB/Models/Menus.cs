namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;
    using FluentNHibernate.Mapping;
    using KitchenPC.Menus;

    public class Menus
    {
        public virtual Guid MenuId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual String Title { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual IList<Favorites> Recipes { get; set; }

        public static Menus FromId(Guid id)
        {
            return new Menus
            {
                MenuId = id
            };
        }

        public static bool operator !=(Menu menu, Menus dbMenu)
        {
            return !(menu == dbMenu);
        }

        public static bool operator ==(Menu menu, Menus dbMenu)
        {
            if (dbMenu == null)
            {
                return !menu.Id.HasValue;
            }

            return menu.Id == dbMenu.MenuId;
        }

        public virtual Menu AsMenu()
        {
            return new Menu(this.MenuId, this.Title);
        }


        public override bool Equals(object obj)
        {
            if (false == (obj is Menus))
            {
                return false;
            }

            var menu = (Menus)obj;
            return this.MenuId.Equals(menu.MenuId);
        }

        public override int GetHashCode()
        {
            return this.MenuId.GetHashCode();
        }
    }

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