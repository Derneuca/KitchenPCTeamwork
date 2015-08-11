namespace KitchenPC.NLP.Tokens
{
    using System.IO;

     public class FormToken : IGrammar
    {
        private static FormSynonyms data;

        /// <summary>
        /// Reads stream to match it against a dictionary of all known forms
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="matchdata"></param>
        /// <returns></returns>
        public bool Read(Stream stream, MatchData matchdata)
        {
            if (data == null)
            {
                data = new FormSynonyms();
            }

            matchdata.Form = null;
            var buffer = string.Empty;
            var matchPos = stream.Position;
            int curByte;

            while ((curByte = stream.ReadByte()) >= 0)
            {
                buffer += (char)curByte;
                FormNode node;
                var match = data.Parse(buffer, out node);
                if (match == MatchPrecision.None)
                {
                    break;
                }

                if (match == MatchPrecision.Exact)
                {
                    matchPos = stream.Position;
                    matchdata.Form = node;
                }
            }

            stream.Seek(matchPos, SeekOrigin.Begin);
            return matchdata.Form != null;
        }
    }
}