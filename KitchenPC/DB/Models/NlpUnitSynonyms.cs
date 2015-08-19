namespace KitchenPC.DB.Models
{
    using System;

    public class NlpUnitSynonyms
    {
        public virtual Guid UnitSynonymId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms Form { get; set; }

        public virtual string Name { get; set; }
    }
}