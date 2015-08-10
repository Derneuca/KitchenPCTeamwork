namespace KitchenPC.Recipes
{
    public class ReadRecipeOptions
    {
        public static readonly ReadRecipeOptions None = new ReadRecipeOptions();
        public static readonly ReadRecipeOptions MethodOnly = new ReadRecipeOptions { ReturnMethod = true };

        public bool ReturnCommentCount { get; set; }

        public bool ReturnUserRating { get; set; }

        public bool ReturnCookbookStatus { get; set; }

        public bool ReturnMethod { get; set; }

        public bool ReturnPermalink { get; set; }
    }
}