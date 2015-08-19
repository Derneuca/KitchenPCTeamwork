namespace KitchenPC.Context
{
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Data;
    using KitchenPC.Ingredients;
    using KitchenPC.NLP;

    public class StaticFormLoader : ISynonymLoader<FormNode>
    {
        private readonly DataStore store;

        public StaticFormLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<FormNode> LoadSynonyms()
        {
            var formSynonyms = this.store.NlpFormSynonyms
               .OrderBy(p => p.Name)
               .Select(s => s.Name)
               .Distinct()
               .ToList();

            var result = new List<FormNode>(formSynonyms.Select(s => new FormNode(s)));
            return result;
        }

        public Pairings LoadFormPairings()
        {
            var forms = this.store.GetIndexedIngredientForms();
            var formSynonyms = this.store.NlpFormSynonyms;
            var pairings = new Pairings();

            foreach (var synonym in formSynonyms)
            {
                var ingredientForm = forms[synonym.FormId];

                string name = synonym.Name;
                var ingredientId = synonym.IngredientId;
                var form = ingredientForm.IngredientFormId;
                var convertionType = ingredientForm.UnitType;
                string displayName = ingredientForm.FormDisplayName;
                string unitName = ingredientForm.UnitName;
                int convertionMultiplier = ingredientForm.ConversionMultiplier;
                float formAmount = ingredientForm.FormAmount;
                var formUnit = ingredientForm.FormUnit;
                var amount = new Amount(formAmount, formUnit);

                pairings.Add(
                   new NameIngredientPair(name, ingredientId),
                   new IngredientForm(form, ingredientId, convertionType, displayName, unitName, convertionMultiplier, amount));
            }

            return pairings;
        }
    }
}