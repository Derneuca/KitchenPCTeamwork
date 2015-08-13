namespace KitchenPC.Data.DTO
{
    using System;

    public class NlpFormSynonyms
    {
        public string Name { get; set; }

        public Guid FormSynonymId { get; set; }

        public Guid IngredientId { get; set; }

        public Guid FormId { get; set; }
    }
}