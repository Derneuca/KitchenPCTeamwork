namespace KitchenPC.Fluent.Menus
{
    using System.Collections.Generic;

    using KitchenPC.Menus;
    using KitchenPC.Recipes;

    /// <summary>Represents one or more recipes to be moved from one menu to another.</summary>
    public class MenuMover
    {
        public MenuMover()
        {
            this.Recipes = new List<Recipe>();
        }

        // Recipes to move
        public IList<Recipe> Recipes { get; set; }

        // Menu to move recipes to
        public Menu TargetMenu { get; set; }

        // Move all recipes in source menu to targetMenu
        public bool MoveAll { get; set; }

        public static MoveAction Create()
        {
            var mover = new MenuMover();
            var result = new MoveAction(mover);
            return result;
        }
    }
}