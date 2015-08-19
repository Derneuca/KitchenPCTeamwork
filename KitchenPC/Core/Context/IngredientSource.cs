namespace KitchenPC.Context
{
    using System;

    public class IngredientSource
    {
        public IngredientSource(Guid id, string displayName)
        {
            this.Id = id;
            this.DisplayName = displayName;
        }

        public Guid Id { get; private set; }

        public string DisplayName { get; private set; }
    }
}