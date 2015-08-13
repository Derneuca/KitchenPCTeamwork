namespace KitchenPC.Data.DTO
{
    using System;

    public class NlpUnitSynonyms
    {
        public string Name { get; set; }

        public Guid UnitSynonymId { get; set; }

        public Guid IngredientId { get; set; }

        public Guid FormId { get; set; }
    }
}