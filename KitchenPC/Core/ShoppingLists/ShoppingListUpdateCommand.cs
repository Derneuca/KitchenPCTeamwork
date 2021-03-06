﻿namespace KitchenPC.ShoppingLists
{
    using System;

    using KitchenPC.Ingredients;
    using KitchenPC.Recipes;

    public class ShoppingListUpdateCommand
    {
        public ShoppingListUpdateType Command { get; set; }

        public Recipe NewRecipe { get; set; }

        public Ingredient NewIngredient { get; set; }

        public IngredientUsage NewUsage { get; set; }

        public string NewRaw { get; set; }

        public Guid? RemoveItem { get; set; }

        public ShoppingListModification ModifyItem { get; set; }
    }
}