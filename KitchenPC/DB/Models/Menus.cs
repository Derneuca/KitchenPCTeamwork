namespace KitchenPC.DB.Models
{
    using System;
    using System.Collections.Generic;

    using KitchenPC.Menus;

    public class Menus
    {
        public virtual Guid MenuId { get; set; }

        public virtual Guid UserId { get; set; }

        public virtual string Title { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual IList<Favorites> Recipes { get; set; }

        public static Menus FromId(Guid id)
        {
            return new Menus
            {
                MenuId = id
            };
        }

        public static bool operator !=(Menu menu, Menus menus)
        {
            return !(menu == menus);
        }

        public static bool operator ==(Menu menu, Menus menus)
        {
            if (menus == null)
            {
                return !menu.Id.HasValue;
            }

            return menu.Id == menus.MenuId;
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
}