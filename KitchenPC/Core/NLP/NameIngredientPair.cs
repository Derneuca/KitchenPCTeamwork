namespace KitchenPC.NLP
{
    using System;

    public class NameIngredientPair
    {
        private readonly int hash;

        public NameIngredientPair(string name, Guid id)
        {
            this.Name = name;
            this.IngredientId = id;
            this.hash = (name + id.ToString()).GetHashCode();
        }

        public string Name { get; set; }

        public Guid IngredientId { get; set; }

        public override int GetHashCode()
        {
            return this.hash;
        }

        public override bool Equals(object obj)
        {
            var pair = obj as NameIngredientPair;

            if (obj == null)
            {
                return false;
            }

            return this.Name == pair.Name && this.IngredientId == pair.IngredientId;
        }
    }
}