namespace KitchenPC.Ingredients
{
    using System;

    public class Ingredient
    {
        public Ingredient(Guid id, string name, IngredientMetadata metadata)
        {
            this.Id = id;
            this.Name = name;
            this.Metadata = metadata;
        }

        public Ingredient(Guid id, string name)
            : this(id, name, new IngredientMetadata())
        {
        }

        public Ingredient()
            : this(Guid.Empty, string.Empty)
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public UnitType ConversionType { get; set; }

        public string UnitName { get; set; }

        public Weight UnitWeight { get; set; }

        public IngredientMetadata Metadata { get; set; }

        public static Ingredient FromId(Guid ingredientId)
        {
            return new Ingredient
            {
                Id = ingredientId
            };
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var ingredient = obj as Ingredient;
            return ingredient != null && this.Id == ingredient.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}