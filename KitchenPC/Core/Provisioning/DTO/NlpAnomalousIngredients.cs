namespace KitchenPC.Data.DTO
{
    using System;

    public class NlpAnomalousIngredients
    {
        public string Name { get; set; }

        public Guid AnomalousIngredientId { get; set; }

        public Guid IngredientId { get; set; }

        public Guid? WeightFormId { get; set; }

        public Guid? VolumeFormId { get; set; }

        public Guid? UnitFormId { get; set; }
    }
}