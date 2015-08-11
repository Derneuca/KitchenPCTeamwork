namespace KitchenPC.ShoppingLists
{
    using System;

    using KitchenPC.Ingredients;
    using KitchenPC.Recipes;

    public class ShoppingListItem : IngredientAggregation
    {
        private string raw;

        public ShoppingListItem(Guid id)
            : base(null)
        {
            this.Id = id;
        }

        public ShoppingListItem(string raw)
            : base(null)
        {
            this.Raw = raw;
        }

        public ShoppingListItem(Ingredient ingredient)
            : base(ingredient)
        {
        }

        public string Raw
        {
            get
            {
                return this.raw;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Shopping list item cannot be blank.");
                }

                this.raw = value;
            }
        }

        public Guid? Id { get; set; }

        public RecipeBrief Recipe { get; set; }

        public bool CrossedOut { get; set; }

        public static ShoppingListItem FromId(Guid id)
        {
            return new ShoppingListItem(id);
        }

        public static implicit operator ShoppingListItem(string s)
        {
            return new ShoppingListItem(s);
        }

        public static implicit operator string(ShoppingListItem item)
        {
            return item.ToString();
        }

        public override IngredientAggregation AddUsage(IngredientUsage usage)
        {
            if (this.Ingredient == null)
            {
                throw new ArgumentException("Cannot add usage to a non-resolved shopping list item.  Create a new shopping list based on an IngredientUsage.");
            }

            return base.AddUsage(usage);
        }

        public override ShoppingListItem GetItem()
        {
            return this;
        }

        public override string ToString()
        {
            string result = !string.IsNullOrEmpty(this.Raw) ? this.Raw : base.ToString();
            return result;
        }

        public override bool Equals(object obj)
        {
            var item = obj as ShoppingListItem;
            if (item == null)
            {
                return false;
            }

            // If they both represent an ingredient, compare by ingredient
            if (this.Ingredient != null && item.Ingredient != null)
            {
                return this.Ingredient.Equals(item.Ingredient);
            }

            // Compare by Raw string
            return string.Equals(this.Raw, item.Raw, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            int result = this.Ingredient != null ? this.Ingredient.Id.GetHashCode() : this.Raw.GetHashCode();
            return result;
        }
    }
}