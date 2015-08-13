namespace KitchenPC.NLP
{
    using System.Collections.Generic;
    using Enums;

    public abstract class SynonymTree<T>
    {
        private static AlphaTree<T> index;

        private static Dictionary<string, T> synonymMap;

        public static AlphaTree<T> Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        public static Dictionary<string, T> SynonymMap
        {
            get
            {
                return synonymMap;
            }

            set
            {
                synonymMap = value;
            }
        }

        public MatchPrecision Parse(string substr, out T match)
        {
            substr = substr.TrimStart(' ');

            if (synonymMap.TryGetValue(substr, out match))
            {
                return MatchPrecision.Exact;
            }

            var node = Index.Head;

            foreach (var t in substr)
            {
                if (node.HasLink(t) == false)
                {
                    return MatchPrecision.None;
                }

                node = node.GetLink(t);
            }

            return MatchPrecision.Partial;
        }

        protected static void IndexString(string value, T node)
        {
            var parsedIng = value.Trim().ToLower();

            if (synonymMap.ContainsKey(parsedIng))
            {
                Parser.Log.Error(string.Format("The ingredient synonym '{0}' also exists as a root ingredient.", value));
            }
            else
            {
                synonymMap.Add(parsedIng, node);
            }

            var curNode = Index.Head;
            foreach (var c in parsedIng)
            {
                if (curNode.HasLink(c) == false)
                {
                    curNode = curNode.AddLink(c);
                }
                else
                {
                    curNode = curNode.GetLink(c);
                }

                curNode.AddConnection(node);
            }
        }
    }
}