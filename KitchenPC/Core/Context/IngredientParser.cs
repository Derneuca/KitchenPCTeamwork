namespace KitchenPC.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class IngredientParser
    {
        private const int MinimumSubstring = 3;

        private Dictionary<Guid, string> nameLookup;
        private AlphaTree index;

        public IEnumerable<IngredientNode> MatchIngredient(string query)
        {
            ConnectorVertex connections;
            if (this.FindSubstring(query, out connections))
            {
                if (connections != null)
                {
                    return connections.Connections;
                }
            }

            return Enumerable.Empty<IngredientNode>();
        }

        public string GetIngredientById(Guid id)
        {
            if (this.nameLookup == null)
            {
                throw new IngredientMapNotInitializedException();
            }

            string ingredient;
            if (this.nameLookup.TryGetValue(id, out ingredient) == false)
            {
                throw new IngredientMapInvalidIngredientException();
            }

            return ingredient;
        }

        public void CreateIndex(IEnumerable<IngredientSource> dataSource)
        {
            this.nameLookup = new Dictionary<Guid, string>();
            this.index = new AlphaTree();

            foreach (var ingredient in dataSource)
            {
                if (string.IsNullOrWhiteSpace(ingredient.DisplayName))
                {
                    continue;
                }

                this.ParseString(ingredient.DisplayName, ingredient.Id);
                this.nameLookup.Add(ingredient.Id, ingredient.DisplayName);
            }
        }

        private void ParseString(string ingredient, Guid id)
        {
            int start = 0;
            int length = MinimumSubstring;
            var node = new IngredientNode(id, ingredient, 0);
            string parsedIngredient = Regex.Replace(ingredient.ToLower(), "[^a-z]", string.Empty);

            while (start + length <= parsedIngredient.Length)
            {
                string substring = parsedIngredient.Substring(start, length);
                this.IndexSubstring(substring, node);
                length++;

                if (start + length > parsedIngredient.Length)
                {
                    start++;
                    length = MinimumSubstring;
                }
            }
        }

        private void IndexSubstring(string substring, IngredientNode node)
        {
            var currentNode = this.index.Head;

            for (int i = 0; i < substring.Length; i++)
            {
                char character = substring[i];
                if (currentNode.HasLink(character) == false)
                {
                    currentNode = currentNode.AddLink(character);
                }
                else
                {
                    currentNode = currentNode.GetLink(character);
                }

                currentNode.AddConnection(node);
            }
        }

        private bool FindSubstring(string substring, out ConnectorVertex connections)
        {
            var node = this.index.Head;
            connections = null;
            substring = Regex.Replace(substring, "[^a-z]", string.Empty);

            for (int i = 0; i < substring.Length; i++)
            {
                if (node.HasLink(substring[i]) == false)
                {
                    return false;
                }

                node = node.GetLink(substring[i]);
            }

            connections = node.Connections;
            return true;
        }
    }
}