namespace KitchenPC.NLP
{
    using System.Collections.Generic;

    public class Anomalies : SynonymTree<AnomalousNode>
    {
        private static readonly object MapInitLock = new object();

        public static void InitIndex(ISynonymLoader<AnomalousNode> loader)
        {
            lock (MapInitLock)
            {
                Index = new AlphaTree<AnomalousNode>();
                SynonymMap = new Dictionary<string, AnomalousNode>();
                var anomalies = loader.LoadSynonyms();

                foreach (var anom in anomalies)
                {
                    IndexString(anom.Name, anom);
                }
            }
        }
    }
}