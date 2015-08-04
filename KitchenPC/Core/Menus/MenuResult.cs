namespace KitchenPC.Menus
{
    using System;

    public class MenuResult
    {
        public bool MenuCreated { get; set; }

        public bool MenuUpdated { get; set; }

        public Guid? NewMenuId { get; set; }
    }
}