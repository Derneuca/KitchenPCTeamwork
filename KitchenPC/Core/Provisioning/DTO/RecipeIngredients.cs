namespace KitchenPC.Data.DTO
{
    using System;

    public class RecipeIngredients
    {
        public Guid RecipeIngredientId { get; set; }

        public Guid RecipeId { get; set; }

        public Guid IngredientId { get; set; }

        public Guid? IngredientFormId { get; set; }

        public Units Unit { get; set; }

        public short DisplayOrder { get; set; }

        public float? QtyLow { get; set; }

        public float? Qty { get; set; }

        public string PreparationNote { get; set; }

        public string Section { get; set; }
    }
}