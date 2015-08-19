namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using KitchenPC.Data;
    using KitchenPC.Data.DTO;
    using KitchenPC.Ingredients;
    using KitchenPC.NLP;

    public class StaticIngredientLoader : ISynonymLoader<NLP.IngredientNode>
    {
        private readonly DataStore store;

        public StaticIngredientLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<NLP.IngredientNode> LoadSynonyms()
        {
            var nodes = new Dictionary<Guid, NLP.IngredientNode>();

            var forms = this.store.GetIndexedIngredientForms();
            var ingredientsForNlp = this.store.Ingredients;
            var pairingMap = this.store.NlpDefaultPairings.ToDictionary(p => p.IngredientId);

            foreach (var ingredient in ingredientsForNlp)
            {
                var ingredientId = ingredient.IngredientId;
                string name = ingredient.DisplayName;
                var convertionType = ingredient.ConversionType;
                Weight unitWeight = ingredient.UnitWeight;
                var pairings = new DefaultPairings();

                NlpDefaultPairings defaultPairing;
                if (pairingMap.TryGetValue(ingredientId, out defaultPairing))
                {
                    if (defaultPairing.WeightFormId.HasValue)
                    {
                        var weightForm = forms[defaultPairing.WeightFormId.Value];
                        var weightFormAmount = new Amount(weightForm.FormAmount, weightForm.FormUnit);
                        pairings.Weight = new IngredientForm(
                            weightForm.IngredientFormId, 
                            ingredientId, 
                            Units.Ounce,
                            null, 
                            null, 
                            weightForm.ConversionMultiplier, 
                            weightFormAmount);
                    }

                    if (defaultPairing.VolumeFormId.HasValue)
                    {
                        var volumeForm = forms[defaultPairing.VolumeFormId.Value];
                        var volumeFormAmount = new Amount(volumeForm.FormAmount, volumeForm.FormUnit);
                        pairings.Volume = new IngredientForm(
                            volumeForm.IngredientFormId, 
                            ingredientId, 
                            Units.Cup, 
                            null, 
                            null, 
                            volumeForm.ConversionMultiplier, 
                            volumeFormAmount);
                    }

                    if (defaultPairing.UnitFormId.HasValue)
                    {
                        var unitForm = forms[defaultPairing.UnitFormId.Value];
                        var unitFormAmount = new Amount(unitForm.FormAmount, unitForm.FormUnit);
                        pairings.Unit = new IngredientForm(
                            unitForm.IngredientFormId,
                            ingredientId,
                            Units.Unit,
                            null, 
                            null, 
                            unitForm.ConversionMultiplier, 
                            unitFormAmount);
                    }
                }

                if (nodes.ContainsKey(ingredientId))
                {
                    Parser.Log.ErrorFormat("[NLP Loader] Duplicate ingredient key due to bad DB data: {0} ({1})", name, ingredientId);
                }
                else
                {
                    nodes.Add(ingredientId, new NLP.IngredientNode(ingredientId, name, convertionType, unitWeight, pairings));
                }
            }

            // Load synonyms
            var ingredientSynonyms = this.store.NlpIngredientSynonyms;

            var result = new List<NLP.IngredientNode>();
            foreach (var synonym in ingredientSynonyms)
            {
                var ingredientId = synonym.IngredientId;
                string alias = synonym.Alias;
                string preparationNote = synonym.PreparationNote;

                NLP.IngredientNode node;
                if (nodes.TryGetValue(ingredientId, out node)) 
                {
                    // TODO: If this fails, maybe throw an exception?
                    result.Add(new NLP.IngredientNode(node, alias, preparationNote));
                }
            }

            result.AddRange(nodes.Values);

            return result;
        }

        public Pairings LoadFormPairings()
        {
            throw new NotImplementedException();
        }
    }
}