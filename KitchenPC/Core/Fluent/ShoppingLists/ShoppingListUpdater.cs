namespace KitchenPC.Fluent.ShoppingLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.Exceptions;
    using KitchenPC.ShoppingLists;

    public class ShoppingListUpdater
    {
        private readonly IKPCContext context;
        private readonly ShoppingList list;
        private readonly IList<ShoppingListAdder> addQueue;
        private readonly IList<ShoppingListItemUpdater> updateQueue;
        private readonly IList<ShoppingListItem> removeQueue;
        private string newName;

        public ShoppingListUpdater(IKPCContext context, ShoppingList list)
        {
            this.context = context;
            this.list = list;
            this.addQueue = new List<ShoppingListAdder>();
            this.updateQueue = new List<ShoppingListItemUpdater>();
            this.removeQueue = new List<ShoppingListItem>();
        }

        public ShoppingListUpdater Rename(string newname)
        {
            if (this.list == ShoppingList.Default)
            {
                throw new FluentExpressionException("Cannot rename default shopping list.");
            }

            this.newName = newname;
            return this;
        }

        public ShoppingListUpdater AddItems(Func<ShoppingListAddAction, ShoppingListAddAction> addAction)
        {
            var action = ShoppingListAdder.Create();
            var result = addAction(action);
            this.addQueue.Add(result.Adder);
            return this;
        }

        public ShoppingListUpdater AddItems(ShoppingListAdder adder)
        {
            this.addQueue.Add(adder);
            return this;
        }

        public ShoppingListUpdater UpdateItem(ShoppingListItem item, Func<ShoppingListItemUpdateAction, ShoppingListItemUpdateAction> updateAction)
        {
            var action = ShoppingListItemUpdater.Create(item);
            var result = updateAction(action);
            this.updateQueue.Add(result.Updater);
            return this;
        }

        public ShoppingListUpdater UpdateItem(ShoppingListItem item, ShoppingListItemUpdater updater)
        {
            this.updateQueue.Add(updater);
            return this;
        }

        public ShoppingListUpdater RemoveItem(ShoppingListItem item)
        {
            this.removeQueue.Add(item);
            return this;
        }

        public ShoppingListResult Commit()
        {
            // Build ShoppingListUpdateCommand array with each update
            var updates = new List<ShoppingListUpdateCommand>();

            if (this.addQueue.Any())
            {
                // Grab new raw entries
                var newRaws = this.addQueue
                    .SelectMany(i => i.ToParse)
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Select(i => new ShoppingListUpdateCommand
                    {
                        Command = ShoppingListUpdateType.AddItem,
                        NewRaw = i
                    });

                // Grab new Recipes
                var newRecipes = this.addQueue
                    .SelectMany(i => i.Recipes)
                    .Where(i => i != null)
                    .Select(i => new ShoppingListUpdateCommand
                    {
                        Command = ShoppingListUpdateType.AddItem,
                        NewRecipe = i
                    });

                // Grab new IngredientUsages
                var newUsages = this.addQueue
                    .SelectMany(i => i.Usages)
                    .Where(i => i != null)
                    .Select(i => new ShoppingListUpdateCommand
                    {
                        Command = ShoppingListUpdateType.AddItem,
                        NewUsage = i
                    });

                // Grab new Ingredients
                var newIngredients = this.addQueue
                    .SelectMany(i => i.Ingredients)
                    .Where(i => i != null)
                    .Select(i => new ShoppingListUpdateCommand
                    {
                        Command = ShoppingListUpdateType.AddItem,
                        NewIngredient = i
                    });

                updates.AddRange(newRaws);
                updates.AddRange(newRecipes);
                updates.AddRange(newUsages);
                updates.AddRange(newIngredients);
            }

            if (this.removeQueue.Any())
            {
                updates.AddRange(this.removeQueue.Where(i => i.Id.HasValue).Select(i => new ShoppingListUpdateCommand
                {
                    Command = ShoppingListUpdateType.RemoveItem,
                    RemoveItem = i.Id
                }));
            }

            if (this.updateQueue.Any())
            {
                updates.AddRange(this.updateQueue.Where(i => i.Item.Id.HasValue).Select(i => new ShoppingListUpdateCommand
                {
                    Command = ShoppingListUpdateType.ModifyItem,
                    ModifyItem = new ShoppingListModification(i.Item.Id.Value, i.NewAmount, i.CrossedOut)
                }));
            }

            var result = this.context.UpdateShoppingList(this.list, updates.ToArray(), this.newName);
            return result;
        }
    }
}
