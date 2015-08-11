namespace KitchenPC.ShoppingLists
{
    public class GetShoppingListOptions
    {
        public static readonly GetShoppingListOptions None = new GetShoppingListOptions();
        public static readonly GetShoppingListOptions WithItems = new GetShoppingListOptions(true);

        public GetShoppingListOptions(bool loadItems = false)
        {
            this.LoadItems = loadItems;
        }

        public bool LoadItems { get; private set; }
    }
}