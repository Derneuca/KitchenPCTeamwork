namespace KitchenPC.Fluent.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context;
    using KitchenPC.Menus;
    using KitchenPC.Recipes;

    /// <summary>Represents a menu to be updated.</summary>
    public class MenuUpdater
    {
        private readonly IKPCContext context;
        private readonly Menu menu;
        private readonly IList<Recipe> addQueue;
        private readonly IList<Recipe> removeQueue;
        private readonly IList<MenuMover> moveQueue;
        private string newTitle;
        private bool clearAll;

        public MenuUpdater(IKPCContext context, Menu menu)
        {
            this.context = context;
            this.menu = menu;
            this.addQueue = new List<Recipe>();
            this.removeQueue = new List<Recipe>();
            this.moveQueue = new List<MenuMover>();
        }

        public MenuUpdater Clear
        {
            get
            {
                this.clearAll = true;
                return this;
            }
        }

        public MenuUpdater Add(Recipe recipe)
        {
            if (!this.addQueue.Contains(recipe))
            {
                this.addQueue.Add(recipe);
            }

            return this;
        }

        public MenuUpdater Remove(Recipe recipe)
        {
            if (!this.removeQueue.Contains(recipe))
            {
                this.removeQueue.Add(recipe);
            }

            return this;
        }

        public MenuUpdater Rename(string newTitle)
        {
            this.newTitle = newTitle;
            return this;
        }

        public MenuUpdater Move(Func<MoveAction, MoveAction> moveAction)
        {
            var action = MenuMover.Create();
            var result = moveAction(action);
            this.moveQueue.Add(result.Mover);
            return this;
        }

        public MenuUpdater Move(MenuMover mover)
        {
            this.moveQueue.Add(mover);
            return this;
        }

        public MenuResult Commit()
        {
            var addRecipes = this.addQueue.Select(r => r.Id).ToArray();
            var removeRecipes = this.removeQueue.Select(r => r.Id).ToArray();
            var moveRecipes = this.moveQueue.Select(m => new MenuMove
                {
                    MoveAll = m.MoveAll,
                    RecipesToMove = m.Recipes.Select(r => r.Id).ToArray(),
                    TargetMenu = m.TargetMenu.Id
                }).ToArray();

            return this.context.UpdateMenu(
                this.menu.Id,
                addRecipes,
                removeRecipes,
                moveRecipes,
                this.clearAll,
                this.newTitle);
        }
    }
}
