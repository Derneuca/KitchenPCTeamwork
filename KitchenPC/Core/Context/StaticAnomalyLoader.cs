namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using KitchenPC.Data;
    using KitchenPC.Ingredients;
    using KitchenPC.NLP;

    public class StaticAnomalyLoader : ISynonymLoader<AnomalousNode>
    {
        private readonly DataStore store;

        public StaticAnomalyLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<AnomalousNode> LoadSynonyms()
        {
            var forms = this.store.GetIndexedIngredientForms();
            var ingredients = this.store.GetIndexedIngredients();
            var anomalies = this.store.NlpAnomalousIngredients;

            var result = new List<AnomalousNode>();

            foreach (var anomaly in anomalies)
            {
                var ingredient = ingredients[anomaly.IngredientId];

                string name = anomaly.Name;
                var ingredientId = anomaly.IngredientId;
                var ingredientName = ingredient.DisplayName;
                IngredientForm weightForm = null;
                IngredientForm volumeForm = null; 
                IngredientForm unitForm = null;

                if (anomaly.WeightFormId.HasValue)
                {
                    var wf = forms[anomaly.WeightFormId.Value];

                    weightForm = new IngredientForm(
                       wf.IngredientFormId,
                       ingredientId,
                       wf.UnitType,
                       wf.FormDisplayName,
                       wf.UnitName,
                       wf.ConversionMultiplier,
                       new Amount(wf.FormAmount, wf.FormUnit));
                }

                if (anomaly.VolumeFormId.HasValue)
                {
                    var vf = forms[anomaly.VolumeFormId.Value];

                    volumeForm = new IngredientForm(
                       vf.IngredientFormId,
                       ingredientId,
                       vf.UnitType,
                       vf.FormDisplayName,
                       vf.UnitName,
                       vf.ConversionMultiplier,
                       new Amount(vf.FormAmount, vf.FormUnit));
                }

                if (anomaly.UnitFormId.HasValue)
                {
                    var uf = forms[anomaly.UnitFormId.Value];

                    unitForm = new IngredientForm(
                       uf.IngredientFormId,
                       ingredientId,
                       uf.UnitType,
                       uf.FormDisplayName,
                       uf.UnitName,
                       uf.ConversionMultiplier,
                       new Amount(uf.FormAmount, uf.FormUnit));
                }

                var pairings = new DefaultPairings() 
                { 
                    Weight = weightForm, 
                    Volume = volumeForm, 
                    Unit = unitForm 
                };

                var ingredientNode = new AnomalousIngredientNode(ingredientId, ingredientName, UnitType.Unit, 0, pairings); // TODO: Must load conv type and unit weight
                result.Add(new AnomalousNode(name, ingredientNode));
            }

            return result;
        }

        public Pairings LoadFormPairings()
        {
            throw new NotImplementedException();
        }
    }
}