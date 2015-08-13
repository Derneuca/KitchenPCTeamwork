namespace KitchenPC.Fluent.ShoppingLists
{
    /// <summary>Provides the ability to fluently express a ShoppingListItemUpdater object.</summary>
    public class ShoppingListItemUpdateAction
    {
        private readonly ShoppingListItemUpdater updater;

        public ShoppingListItemUpdateAction(ShoppingListItemUpdater updater)
        {
            this.updater = updater;
        }

        public ShoppingListItemUpdater Updater
        {
            get
            {
                return this.updater;
            }
        }

        public ShoppingListItemUpdateAction CrossOut
        {
            get
            {
                this.updater.CrossedOut = true;
                return this;
            }
        }

        public ShoppingListItemUpdateAction UnCrossOut
        {
            get
            {
                this.updater.CrossedOut = false;
                return this;
            }
        }

        public ShoppingListItemUpdateAction NewAmount(Amount newAmount)
        {
            this.updater.NewAmount = newAmount;
            return this;
        }
    }
}
