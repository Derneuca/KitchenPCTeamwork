namespace KitchenPC.Data.DTO
{
    using System;

    public class NlpIngredientSynonyms
    {
        public Guid IngredientSynonymId { get; set; }

        public Guid IngredientId { get; set; }

        public string Alias { get; set; }

        public string PreparationNote { get; set; }
    }
}