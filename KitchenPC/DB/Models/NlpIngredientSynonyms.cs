namespace KitchenPC.DB.Models
{
    using System;

    public class NlpIngredientSynonyms
    {
        public virtual Guid IngredientSynonymId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual string Alias { get; set; }

        public virtual string Prepnote { get; set; }
    }
}