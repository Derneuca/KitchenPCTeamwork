namespace KitchenPC.DB.Models
{
    using System;

    public class NlpDefaultPairings
    {
        public virtual Guid DefaultPairingId { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms WeightForm { get; set; }

        public virtual IngredientForms VolumeForm { get; set; }

        public virtual IngredientForms UnitForm { get; set; }
    }
}