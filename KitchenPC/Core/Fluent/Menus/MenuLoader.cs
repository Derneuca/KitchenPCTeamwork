namespace KitchenPC.Fluent.Menus
{
    using System.Collections.Generic;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Menus;

    /// <summary>Represents one or more menus to be loaded.</summary>
    public class MenuLoader
    {
        private readonly IKPCContext context;
        private readonly IList<Menu> menusToLoad;
        private readonly bool loadAll;
        private bool loadRecipes;

        public MenuLoader(IKPCContext context)
        {
            this.context = context;
            this.loadAll = true;
        }

        public MenuLoader(IKPCContext context, Menu menu)
        {
            this.context = context;
            this.menusToLoad = new List<Menu>() { menu };
        }

        public MenuLoader WithRecipes
        {
            get
            {
                // BUGBUG: Technically, we should be creating a new instance and copying the menus over, as there might
                // be a reference to the old chain somewhere that we'd be updating.
                this.loadRecipes = true;
                return this;
            }
        }

        public MenuLoader Load(Menu menu)
        {
            if (this.loadAll)
            {
                throw new FluentExpressionException("To specify individual menus to load, remove the LoadAll clause from your expression.");
            }

            this.menusToLoad.Add(menu);
            return this;
        }

        public IList<Menu> List()
        {
            var options = new GetMenuOptions();
            options.LoadRecipes = this.loadRecipes;
            var result = this.context.GetMenus(this.menusToLoad, options);
            return result;
        }
    }
}
