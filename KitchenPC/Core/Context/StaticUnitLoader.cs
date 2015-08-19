namespace KitchenPC.Context
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Data;
    using KitchenPC.Ingredients;
    using KitchenPC.NLP;

    public class StaticUnitLoader : ISynonymLoader<UnitNode>
    {
        private readonly DataStore store;

        public StaticUnitLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<UnitNode> LoadSynonyms()
        {
            var unitSynonyms = this.store.NlpUnitSynonyms
               .OrderBy(p => p.Name)
               .Select(p => p.Name)
               .Distinct()
               .ToList();

            var result = new List<CustomUnitNode>(unitSynonyms.Select(s => new CustomUnitNode(s)));
            return result;
        }

        public Pairings LoadFormPairings()
        {
            var forms = this.store.GetIndexedIngredientForms();
            var unitSynonyms = this.store.NlpUnitSynonyms;
            var pairings = new Pairings();

            foreach (var synonym in unitSynonyms)
            {
                var form = forms[synonym.FormId];

                pairings.Add(
                    new NameIngredientPair(
                        synonym.Name.Trim(), 
                        synonym.IngredientId),
                        new IngredientForm(
                            form.IngredientFormId,
                            form.IngredientId,
                            form.UnitType,
                            form.FormDisplayName,
                            form.UnitName,
                            form.ConversionMultiplier,
                            new Amount(form.FormAmount, form.FormUnit)));
            }

            return pairings;
        }
    }
}