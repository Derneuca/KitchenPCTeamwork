namespace KitchenPC.NLP
{
    using System.Collections.Generic;

    public class IngredientSynonyms : SynonymTree<IngredientNode>
    {
        private static readonly object MapInitLock = new object();

        public static void InitIndex(ISynonymLoader<IngredientNode> loader)
        {
            lock (MapInitLock)
            {
                Index = new AlphaTree<IngredientNode>();
                SynonymMap = new Dictionary<string, IngredientNode>();
                var ings = loader.LoadSynonyms();

                foreach (var ing in ings)
                {
                    IndexString(ing.IngredientName, ing);
                }
            }
        }
    }
}