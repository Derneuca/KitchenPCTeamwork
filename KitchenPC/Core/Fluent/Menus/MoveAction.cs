namespace KitchenPC.Fluent.Menus
{
    using KitchenPC.Menus;
    using KitchenPC.Recipes;

    /// <summary>Provides the ability to fluently express a MenuMover object.</summary>
    public class MoveAction
    {
        private readonly MenuMover mover;

        public MoveAction(MenuMover mover)
        {
            this.mover = mover;
        }

        public MoveAction AllRecipes
        {
            get
            {
                return new MoveAction(new MenuMover
                {
                    MoveAll = true,
                    TargetMenu = this.mover.TargetMenu
                });
            }
        }

        public MenuMover Mover
        {
            get
            {
                return this.mover;
            }
        }

        public MoveAction Recipe(Recipe recipe)
        {
            this.mover.Recipes.Add(recipe);
            return this;
        }

        public MoveAction To(Menu menu)
        {
            this.mover.TargetMenu = menu;
            return this;
        }
    }
}
