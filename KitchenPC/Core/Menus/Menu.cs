namespace KitchenPC.Menus
{
    using System;

    using KitchenPC.Recipes;

    public class Menu
    {
        public static readonly Menu Favorites = new Menu(null, "Favorites");

        public Menu(Guid? id, string title)
        {
            this.Id = id;
            this.Title = title;
            this.Recipes = null;
        }

        public Menu(Menu menu)
            : this(menu.Id, menu.Title)
        {
            if (menu.Recipes != null)
            {
                this.Recipes = new RecipeBrief[menu.Recipes.Length];
                menu.Recipes.CopyTo(this.Recipes, 0);
            }
        }

        public Guid? Id { get; set; }

        public string Title { get; set; }

        public RecipeBrief[] Recipes { get; set; } // Can be null

        public static Menu FromId(Guid menuId)
        {
            var result = new Menu(menuId, null);
            return result;
        }

        public override string ToString()
        {
            int count = this.Recipes != null ? this.Recipes.Length : 0;

            return string.Format(
                "{0} ({1} {2})",
                this.Title,
                count,
                count != 1 ? "recipes" : "recipe");
        }

        public override bool Equals(object obj)
        {
            bool result;
            if (!(obj is Menu))
            {
                result = false;
                return result;
            }

            var menu = (Menu)obj;
            if (this.Id.HasValue || menu.Id.HasValue)
            {
                result = this.Id.Equals(menu.Id);
                return result;
            }

            result = this.Title.Equals(menu.Title);
            return result;
        }

        public override int GetHashCode()
        {
            int result = this.Id.HasValue ? this.Id.Value.GetHashCode() : this.Title.GetHashCode();
            return result;
        }
    }
}