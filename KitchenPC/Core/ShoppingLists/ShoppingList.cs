namespace KitchenPC.ShoppingLists
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ShoppingList : IEnumerable<ShoppingListItem>
    {
        public static readonly Guid GuidWater = new Guid("cb44df2d-f27c-442a-bd6e-fd7bdd501f10");
        public static readonly ShoppingList Default = new ShoppingList(null, string.Empty);

        private readonly List<ShoppingListItem> list;

        public ShoppingList()
        {
            this.list = new List<ShoppingListItem>();
        }

        public ShoppingList(Guid? id, string title)
            : this()
        {
            this.Id = id;
            this.Title = title;
        }

        public ShoppingList(Guid? id, string title, IEnumerable<IShoppingListSource> items)
            : this(id, title)
        {
            this.AddItems(items);
        }

        public Guid? Id { get; set; }

        public string Title { get; set; }

        public static ShoppingList FromId(Guid menuId)
        {
            return new ShoppingList(menuId, null);
        }

        public void AddItems(IEnumerable<IShoppingListSource> items)
        {
            foreach (var item in items)
            {
                this.AddItem(item.GetItem());
            }
        }

        public override string ToString()
        {
            string title = !string.IsNullOrEmpty(this.Title) ? this.Title : "Default List";
            int count = this.list.Count;
            string result = string.Format(
                "{0} ({1} Item{2})", 
                title, 
                count, 
                count != 1 ? "s" : string.Empty);
            return result;
        }

        public IEnumerator<ShoppingListItem> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        private void AddItem(ShoppingListItem item)
        {
            var existingItem = this.list.Find(i => i.Equals(item));
            if (existingItem == null)
            {
                this.list.Add(item);
                return;
            }

            // If new item is crossed out, cross out existing item
            existingItem.CrossedOut = item.CrossedOut;

            // Adding same ingredient twice, but nothing to aggregate. Skip.
            if (existingItem.Ingredient == null || existingItem.Amount == null)
            {
                return;
            }

            // Clear out existing amount
            if (item.Amount == null)
            {
                existingItem.Amount = null;
                return;
            }

            existingItem.Amount += item.Amount;
        }
    }
}