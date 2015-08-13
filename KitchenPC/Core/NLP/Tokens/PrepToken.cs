namespace KitchenPC.NLP.Tokens
{
    using System.IO;
    using System.Text.RegularExpressions;
    using Enums;

    public class PrepToken : IGrammar
    {
        private static PrepNotes data;

        public bool Read(Stream stream, MatchData matchData)
        {
            if (data == null)
            {
                data = new PrepNotes();
            }

            var buffer = string.Empty;
            PrepNode foundPrep = null;
            var matchFound = false;
            var matchPos = stream.Position;
            int curByte;

            while ((curByte = stream.ReadByte()) >= 0)
            {
                buffer += (char)curByte;

                //Prep tokens can have leading commas or parens - so trim these off
                buffer = Regex.Replace(buffer, @"^\s*(,|-|\()\s*", "");
                buffer = Regex.Replace(buffer, @"\s*\)\s*$", "");

                PrepNode node;
                var match = data.Parse(buffer, out node);
                if (match == MatchPrecision.None)
                {
                    break;
                }

                if (match == MatchPrecision.Exact)
                {
                    matchPos = stream.Position;
                    foundPrep = node;
                    matchFound = true;
                }
            }

            if (foundPrep != null)
            {
                matchData.Preps.Add(foundPrep);
            }

            stream.Seek(matchPos, SeekOrigin.Begin);
            return matchFound;
        }
    }
}