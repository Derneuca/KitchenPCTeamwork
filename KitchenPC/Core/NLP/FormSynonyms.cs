﻿namespace KitchenPC.NLP
{
    using System;
    using System.Collections.Generic;
    using Ingredients;

    public class FormSynonyms : SynonymTree<FormNode>
    {
        private static readonly object MapInitLock = new object();
        private static Pairings pairings;

        public static void InitIndex(ISynonymLoader<FormNode> loader)
        {
            lock (MapInitLock)
            {
                Index = new AlphaTree<FormNode>();
                SynonymMap = new Dictionary<string, FormNode>();

                foreach (var form in loader.LoadSynonyms())
                {
                    IndexString(form.FormName, form);
                }

                pairings = loader.LoadFormPairings();
            }
        }

        public static bool TryGetFormForIngredient(string formname, Guid ing, out IngredientForm form)
        {
            form = null;
            FormNode node;
            if (false == SynonymMap.TryGetValue(formname, out node))
            {
                return false;
            }

            var pair = new NameIngredientPair(formname, ing);
            return pairings.TryGetValue(pair, out form);
        }

        public static bool TryGetFormForPrep(Preps preps, IngredientNode ing, bool remove, out IngredientForm form)
        {
            //TODO: Do we need to check all preps, or just the one that was on the input
            foreach (var prep in preps)
            {
                var matchFound = TryGetFormForIngredient(prep.Prep, ing.Id, out form);
                if (!matchFound)
                {
                    continue;
                }

                if (remove)
                {
                    preps.Remove(prep);
                }

                return true;
            }

            form = null;
            return false;
        }
    }
}