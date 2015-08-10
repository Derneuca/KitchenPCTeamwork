namespace KitchenPC.DB.Models
{
    using System;

    public class NlpFormSynonyms
    {
        public virtual Guid FormSynonymId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms Form { get; set; }

        public virtual string Name { get; set; }
    }
}