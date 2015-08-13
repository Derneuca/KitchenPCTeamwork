namespace KitchenPC.Data.DTO
{
    using System;
    using KitchenPC.Menus;

    public class Menus
    {
        public Guid MenuId { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; }

        public DateTime CreatedDate { get; set; }

        public static Menu ToMenu(Menus dtoMenu)
        {
            var resultMenu = new Menu(dtoMenu.MenuId, dtoMenu.Title);
            return resultMenu;
        }
    }
}