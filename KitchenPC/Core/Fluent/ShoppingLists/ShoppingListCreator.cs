namespace KitchenPC.Fluent.ShoppingLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Context.Interfaces;
    using KitchenPC.ShoppingLists;

    public class ShoppingListCreator
    {
        private readonly IList<ShoppingListAdder> addQueue;
        private readonly IKPCContext context;
        private string listName;

        public ShoppingListCreator(IKPCContext context)
        {
            this.context = context;
            this.addQueue = new List<ShoppingListAdder>();
            this.listName = "New Shopping List";
        }

        public ShoppingListCreator WithName(string name)
        {
            this.listName = name;
            return this;
        }

        public ShoppingListCreator AddItems(Func<ShoppingListAddAction, ShoppingListAddAction> addAction)
        {
            var action = ShoppingListAdder.Create();
            var result = addAction(action);

            this.addQueue.Add(result.Adder);
            return this;
        }

        public ShoppingListCreator AddItems(ShoppingListAdder adder)
        {
            this.addQueue.Add(adder);
            return this;
        }

        public ShoppingListResult Commit()
        {
            var recipes = this.addQueue.SelectMany(r => r.Recipes).ToArray();
            var ingredients = this.addQueue.SelectMany(i => i.Ingredients).ToArray();
            var usages = this.addQueue.SelectMany(u => u.Usages).ToArray();
            var raw = this.addQueue.SelectMany(r => r.ToParse).ToArray();
            var result = this.context.CreateShoppingList(this.listName, recipes, ingredients, usages, raw);
            return result;
        }
    }
}
