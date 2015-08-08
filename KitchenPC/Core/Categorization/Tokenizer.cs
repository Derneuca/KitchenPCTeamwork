namespace KitchenPC.Categorization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using KitchenPC.Recipes;

    // Takes a Recipe object and returns an enumeration of Token objects
    public static class Tokenizer
    {
        // All tokens have to have at least one letter in them
        private static readonly Regex Valid = new Regex(@"[a-z]", RegexOptions.IgnoreCase); 

        public static IEnumerable<IToken> Tokenize(Recipe recipe)
        {
            var tokens = new List<IToken>();
            tokens.AddRange(ParseText(recipe.Title ?? string.Empty));
            tokens.AddRange(ParseText(recipe.Description ?? string.Empty));
            //tokens.AddRange(ParseText(recipe.Method ?? string.Empty));
            //tokens.Add(new TimeToken(recipe.CookTime.GetValueOrDefault() + recipe.PrepTime.GetValueOrDefault()));
            tokens.AddRange(
                from i in recipe.Ingredients.NeverNull() 
                select new IngredientToken(i.Ingredient) as IToken);

            return tokens;
        }

        private static IEnumerable<IToken> ParseText(string text)
        {
            var parts = Regex.Split(text, @"[^a-z0-9\'\$\-]", RegexOptions.IgnoreCase);
            return from p in parts
                   where Valid.IsMatch(p)
                   select new TextToken(p) as IToken;
        }
    }
}