namespace KitchenPC.NLP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Enums;
    using log4net;

    public class Parser
    {
        private static readonly Regex ReWhitespace = new Regex(@"[ ]{2,}", RegexOptions.Compiled);

        private List<Template> templates;

        public delegate void NoMatchEvent(NoMatch result, string usage);

        public static readonly ILog Log = LogManager.GetLogger(typeof(Parser));

        public NoMatchEvent OnNoMatch { get; set; }

        public TemplateStatistics Stats { get; private set; }

        public void LoadTemplates(params Template[] templates)
        {
            this.templates = new List<Template>(templates.Length);
            this.Stats = new TemplateStatistics();

            foreach (var t in templates)
            {
                NlpTracer.Trace(TraceLevel.Debug, "Loaded Template: {0}", t);
                this.templates.Add(t);
                this.Stats.RecordTemplate(t);
            }
        }

        public IEnumerable<Result> ParseAll(params string[] input)
        {
            return input.Select(this.Parse);
        }

        public Result Parse(string input)
        {
            ReplaceAccents(ref input);
            var normalizedInput = ReWhitespace.Replace(input, " ").ToLower();

            // Loop through all loaded templates looking for a match - return that match, or return null if unknown
            var bestResult = MatchResult.NoMatch;
            foreach (var t in this.templates)
            {
                var result = t.Parse(normalizedInput);
                if (result is Match)
                {
                    Stats[t]++;
                    return result;
                }

                if (result.Status > bestResult)
                {
                    bestResult = result.Status;
                }
            }

            NlpTracer.Trace(TraceLevel.Info, "Could not find match for usage: {0}", input);
            var ret = new NoMatch(input, bestResult);
            if (this.OnNoMatch != null)
            {
                this.OnNoMatch(ret, input);
            }

            return ret; // TODO: Save best match to get error code
        }
        private static void ReplaceAccents(ref string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            input = Regex.Replace(input, @"[\xC0-\xC5\xE0-\xE5]", "a");
            input = Regex.Replace(input, @"[\xC8-\xCB\xE8-\xEB]", "e");
            input = Regex.Replace(input, @"[\xCC-\xCF\xEC-\xEF]", "i");
            input = Regex.Replace(input, @"[\xD1\xF1]", "n");
            input = Regex.Replace(input, @"[\xD2-\xD6\xF2-\xF6]", "o");
            input = Regex.Replace(input, @"[\xD9-\xDC\xF9-\xFC]", "u");
            input = Regex.Replace(input, @"[\xDD\xDF\xFF]", "y");
        }
    }
}