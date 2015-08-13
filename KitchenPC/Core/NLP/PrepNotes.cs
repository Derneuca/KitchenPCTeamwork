namespace KitchenPC.NLP
{
    using System.Collections.Generic;

    public class PrepNotes : SynonymTree<PrepNode>
    {
        private static readonly object MapInitLock = new object();

        public static void InitIndex(ISynonymLoader<PrepNode> loader)
        {
            lock (MapInitLock)
            {
                Index = new AlphaTree<PrepNode>();
                SynonymMap = new Dictionary<string, PrepNode>();
                var preps = loader.LoadSynonyms();

                foreach (var prep in preps)
                {
                    IndexString(prep.Prep, prep);
                }
            }
        }
    }
}