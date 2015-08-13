namespace KitchenPC.NLP.Tokens
{
    using System.IO;
    using Enums;

    public class IngredientToken : IGrammar
    {
        private static IngredientSynonyms data;

        /// <summary>
        /// Reads stream to match it against a dictionary of all known ingredients and their aliases
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="matchdata"></param>
        /// <returns></returns>
        public bool Read(Stream stream, MatchData matchdata)
        {
            if (data == null)
            {
                data = new IngredientSynonyms();
            }

            matchdata.Ingredient = null;
            var buffer = string.Empty;
            var matchPos = stream.Position;
            int curByte;

            while ((curByte = stream.ReadByte()) >= 0)
            {
                buffer += (char)curByte;
                IngredientNode node;
                var match = data.Parse(buffer, out node);
                if (match == MatchPrecision.None)
                {
                    break;
                }

                if (match == MatchPrecision.Exact)
                {
                    matchPos = stream.Position;
                    matchdata.Ingredient = node;
                }
            }

            //Add any prep notes from this ingredient alias to the prep node collection
            if (matchdata.Ingredient != null && matchdata.Ingredient.PrepNote != null)
            {
                matchdata.Preps.Add(matchdata.Ingredient.PrepNote);
            }

            stream.Seek(matchPos, SeekOrigin.Begin);
            return matchdata.Ingredient != null;
        }
    }
}