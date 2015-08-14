namespace KitchenPC.Modeler
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a query for the modeler, such as a list of ingredients, recipes to avoid, and a number of recipes to return.
    /// </summary>
    public class ModelerQuery
    {
        // User-entered ingredients to be parsed by NLP
        public string[] Ingredients { get; set; }

        // Avoid specific recipe, useful for swapping out one recipe for another
        public Guid? AvoidRecipe { get; set; }

        public byte NumberOfRecipes { get; set; }

        public byte Scale { get; set; }

        public string CacheKey
        {
            get
            {
                var bytes = new List<byte>();

                // First byte is number of recipes
                bytes.Add(this.NumberOfRecipes);

                // Second byte is the scale
                bytes.Add(this.Scale);

                if (this.AvoidRecipe.HasValue)
                {
                    bytes.AddRange(this.AvoidRecipe.Value.ToByteArray());
                }

                // Remaining bytes are defined ingredients, delimited by null
                if (this.Ingredients != null && this.Ingredients.Length > 0)
                {
                    foreach (var ingredients in this.Ingredients)
                    {
                        bytes.AddRange(Encoding.UTF8.GetBytes(ingredients.ToLower().Trim()));
                        bytes.Add(0); // Null delimiter
                    }
                }

                string cache = Convert.ToBase64String(bytes.ToArray());
                return cache;
            }
        }
    }
}