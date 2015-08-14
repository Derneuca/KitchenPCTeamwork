namespace KitchenPC.Modeler
{
    using System;
    using KitchenPC.Recipes;

    public struct RecipeBinding
    {
        public Guid Id { get; set; }

        public byte Rating { get; set; }

        public RecipeTags Tags { get; set; }

        public bool Hidden { get; set; }
    }
}