namespace KitchenPC.DB.Models
{
    using System;

    public class NlpAnomalousIngredients
    {
        public virtual Guid AnomalousIngredientId { get; set; }

        public virtual string Name { get; set; }

        public virtual Ingredients Ingredient { get; set; }

        public virtual IngredientForms WeightForm { get; set; }

        public virtual IngredientForms VolumeForm { get; set; }

        public virtual IngredientForms UnitForm { get; set; }
    }
}