namespace KitchenPC.NLP
{
    using System.Collections.Generic;
    using Ingredients;

    public class Pairings
    {
        private readonly IDictionary<NameIngredientPair, IngredientForm> pairs;

        public Pairings()
        {
            this.pairs = new Dictionary<NameIngredientPair, IngredientForm>();
        }

        public void Add(NameIngredientPair key, IngredientForm value)
        {
            this.pairs.Add(key, value);
        }

        public bool ContainsKey(NameIngredientPair key)
        {
            return this.pairs.ContainsKey(key);
        }

        public bool TryGetValue(NameIngredientPair key, out IngredientForm value)
        {
            return this.pairs.TryGetValue(key, out value);
        }

        public IngredientForm this[NameIngredientPair key]
        {
            get
            {
                return this.pairs[key];
            }

            set
            {
                this.pairs[key] = value;
            }
        }
    }
}