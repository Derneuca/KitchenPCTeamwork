namespace KitchenPC.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using NLP;

    public class PrepLoader : ISynonymLoader<PrepNode>
    {
        private readonly DatabaseAdapter adapter;

        public PrepLoader(DatabaseAdapter adapter)
        {
            this.adapter = adapter;
        }

        public IEnumerable<PrepNode> LoadSynonyms()
        {
            using (var session = this.adapter.GetStatelessSession())
            {
                var forms = session.QueryOver<NlpFormSynonyms>().Select(p => p.Name).List<string>();
                var preps = session.QueryOver<NlpPrepNotes>().Select(p => p.Name).List<string>();

                var ret = forms
                   .Concat(preps)
                   .Distinct()
                   .Select(p => new PrepNode(p))
                   .ToList();

                return ret;
            }
        }

        public Pairings LoadFormPairings()
        {
            throw new NotImplementedException();
        }
    }
}