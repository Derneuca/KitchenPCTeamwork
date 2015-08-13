namespace KitchenPC.Fluent.Menus
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context;
    using KitchenPC.Menus;

    /// <summary>Represents one or more menus to be deleted.</summary>
    public class MenuDeleter
    {
        private readonly IKPCContext context;
        private readonly IList<Menu> menusToDelete;

        public MenuDeleter(IKPCContext context, Menu menu)
        {
            this.context = context;
            this.menusToDelete = new List<Menu>() { menu };
        }

        public MenuDeleter Delete(Menu menu)
        {
            if (!menu.Id.HasValue)
            {
                throw new MenuIdRequiredException();
            }

            this.menusToDelete.Add(menu);
            return this;
        }

        public void Commit()
        {
            this.context.DeleteMenus(this.menusToDelete.Select(m => m.Id.Value).ToArray());
        }
    }
}
