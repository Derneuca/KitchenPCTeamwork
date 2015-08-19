namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KitchenPC.Data;
    using KitchenPC.NLP;

    public class StaticPrepLoader : ISynonymLoader<PrepNode>
    {
        private readonly DataStore store;

        public StaticPrepLoader(DataStore store)
        {
            this.store = store;
        }

        public IEnumerable<PrepNode> LoadSynonyms()
        {
            var forms = this.store.NlpFormSynonyms.Select(p => p.Name);
            var preps = this.store.NlpPrepNotes.Select(p => p.Name);

            var result = forms
               .Concat(preps)
               .Distinct()
               .Select(p => new PrepNode(p))
               .ToList();

            return result;
        }

        public Pairings LoadFormPairings()
        {
            throw new NotImplementedException();
        }
    }
}