namespace KitchenPC.NLP.Tokens
{
    using System.IO;

    public class AnomalousToken : IGrammar
    {
        private static Anomalies data;

        public bool Read(Stream stream, MatchData matchData)
        {
            if (data == null)
            {
                data = new Anomalies();
            }

            var buffer = string.Empty;
            AnomalousNode foundNode = null;
            var matchFound = false;
            var matchPos = stream.Position;
            int curByte;

            while ((curByte = stream.ReadByte()) >= 0)
            {
                buffer += (char)curByte;

                AnomalousNode node;
                var match = data.Parse(buffer, out node);
                if (match == MatchPrecision.None)
                {
                    break;
                }

                if (match == MatchPrecision.Exact)
                {
                    matchPos = stream.Position;
                    foundNode = node;
                    matchFound = true;
                }
            }

            if (foundNode != null)
            {
                matchData.Ingredient = foundNode.Ingredient;
            }

            stream.Seek(matchPos, SeekOrigin.Begin);
            return matchFound;
        }
    }
}