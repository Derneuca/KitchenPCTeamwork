namespace KitchenPC.DB
{
    using System;
    using System.Collections.Generic;
    using Ingredients;
    using Models;
    using NHibernate.Criterion;
    using NLP;

    public class AnomalyLoader : ISynonymLoader<AnomalousNode>
    {
        private readonly DatabaseAdapter adapter;

        public AnomalyLoader(DatabaseAdapter adapter)
        {
            this.adapter = adapter;
        }

        public IEnumerable<AnomalousNode> LoadSynonyms()
        {
            using (var session = this.adapter.GetStatelessSession())
            {
                var anomalies = session.QueryOver<NlpAnomalousIngredients>()
                   .Fetch(prop => prop.Ingredient).Eager()
                   .Fetch(prop => prop.WeightForm).Eager()
                   .Fetch(prop => prop.VolumeForm).Eager()
                   .Fetch(prop => prop.UnitForm).Eager()
                   .List();

                var ret = new List<AnomalousNode>();

                foreach (var anon in anomalies)
                {
                    var name = anon.Name;
                    var ing = anon.Ingredient.IngredientId;
                    var ingName = anon.Ingredient.DisplayName;

                    IngredientForm weightForm = null, volumeForm = null, unitForm = null;
                    if (anon.WeightForm != null)
                    {
                        weightForm = new IngredientForm(
                           anon.WeightForm.IngredientFormId,
                           ing,
                           anon.WeightForm.UnitType,
                           anon.WeightForm.FormDisplayName,
                           anon.WeightForm.UnitName,
                           anon.WeightForm.ConvMultiplier,
                           new Amount(anon.WeightForm.FormAmount, anon.WeightForm.FormUnit));
                    }

                    if (anon.VolumeForm != null)
                    {
                        volumeForm = new IngredientForm(
                           anon.VolumeForm.IngredientFormId,
                           ing,
                           anon.VolumeForm.UnitType,
                           anon.VolumeForm.FormDisplayName,
                           anon.VolumeForm.UnitName,
                           anon.VolumeForm.ConvMultiplier,
                           new Amount(anon.VolumeForm.FormAmount, anon.VolumeForm.FormUnit));
                    }

                    if (anon.UnitForm != null)
                    {
                        unitForm = new IngredientForm(
                           anon.UnitForm.IngredientFormId,
                           ing,
                           anon.UnitForm.UnitType,
                           anon.UnitForm.FormDisplayName,
                           anon.UnitForm.UnitName,
                           anon.UnitForm.ConvMultiplier,
                           new Amount(anon.UnitForm.FormAmount, anon.UnitForm.FormUnit));
                    }

                    var pairings = new DefaultPairings() { Weight = weightForm, Volume = volumeForm, Unit = unitForm };
                    var ingNode = new AnomalousIngredientNode(ing, ingName, UnitType.Unit, 0, pairings);
                    ret.Add(new AnomalousNode(name, ingNode));
                }

                return ret;
            }
        }

        public Pairings LoadFormPairings()
        {
            throw new NotImplementedException();
        }
    }
}