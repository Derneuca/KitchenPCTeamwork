namespace KitchenPC.ShoppingLists
{
    using System;

    public class ShoppingListModification
    {
        public ShoppingListModification(Guid itemId, Amount newAmount, bool? crossout)
        {
            this.ModifiedItemId = itemId;
            this.NewAmount = newAmount;
            this.CrossOut = crossout;
        }

        public Guid ModifiedItemId { get; private set; }

        public Amount NewAmount { get; private set; }

        public bool? CrossOut { get; private set; }
    }
}