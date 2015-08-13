namespace KitchenPC.NLP.Tokens
{
    using System.IO;
    using Enums;

    public class NumericToken : IGrammar
    {
        static NumericVocab data;

        /// <summary>
        /// Reads stream to match it against a dictionary of all known units for an ingredient
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="matchdata"></param>
        /// <returns></returns>
        public bool Read(Stream stream, MatchData matchdata)
        {
            if (data == null)
            {
                data = new NumericVocab();
            }

            var matchFound = false;
            var buffer = string.Empty;
            var matchPos = stream.Position;
            int curByte;

            while ((curByte = stream.ReadByte()) >= 0)
            {
                buffer += (char)curByte;
                NumericNode node;
                var match = data.Parse(buffer, out node);
                if (match == MatchPrecision.None)
                {
                    stream.Seek(matchPos, SeekOrigin.Begin);
                    break;
                }

                if (match == MatchPrecision.Exact)
                {
                    matchPos = stream.Position;
                    matchFound = true;
                    matchdata.Amount = new Amount { SizeHigh = node.Value };
                }
            }

            return matchFound;
        }
    }
}