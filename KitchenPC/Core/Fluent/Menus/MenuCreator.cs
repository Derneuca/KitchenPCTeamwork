namespace KitchenPC.Fluent.Menus
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context;
    using KitchenPC.Menus;
    using KitchenPC.Recipes;
   
    /// <summary>Represents a menu to be created.</summary>
    public class MenuCreator
    {
        private readonly IKPCContext context;
        private readonly IList<Recipe> recipes;
        private string title;

        public MenuCreator(IKPCContext context)
        {
            this.context = context;
            this.recipes = new List<Recipe>();
            this.title = "New Menu";
        }

        public MenuCreator WithTitle(string name)
        {
            this.title = name;
            return this;
        }

        public MenuCreator AddRecipe(Recipe recipe)
        {
            this.recipes.Add(recipe);
            return this;
        }

        public MenuResult Commit()
        {
            var newMenu = new Menu(null, this.title);
            var result = this.context.CreateMenu(newMenu, this.recipes.Select(r => r.Id).ToArray());
            return result;
        }
    }
}
