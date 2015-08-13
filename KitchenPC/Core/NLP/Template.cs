namespace KitchenPC.NLP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Enums;
    using Tokens;

    public class Template : IComparable<string>, IEquatable<string>
    {
        private readonly LinkedList<IGrammar> grammar;
        private readonly string template;

        public Template(string template)
        {
            this.grammar = new LinkedList<IGrammar>();
            this.CompileTemplate(template);
            this.template = template;
        }

        public string DefaultPrep { get; set; }

        public bool AllowPartial { get; set; }

        public Result Parse(string usage)
        {
            var matchdata = new MatchData();
            var stream = new MemoryStream(Encoding.Default.GetBytes(usage), false);

            if (this.grammar.Any(g => false == g.Read(stream, matchdata)))
            {
                return new NoMatch(usage, MatchResult.NoMatch);
            }

            if (stream.ReadByte() >= 0)
            {
                return new NoMatch(usage, MatchResult.NoMatch); // TODO: If no other templates match, this might be our best bet - store closest match and return that.
            }

            NlpTracer.Trace(TraceLevel.Info, "Usage \"{0}\" matches the grammar \"{1}\"", usage, this);
            var ret = Result.BuildResult(this, usage, matchdata);
            return ret;
        }

        public int CompareTo(string other)
        {
            return string.Compare(this.template, other, StringComparison.Ordinal);
        }

        public bool Equals(string other)
        {
            return this.template.Equals(other);
        }

        public static implicit operator Template(string t)
        {
            return new Template(t);
        }

        public static implicit operator string (Template t)
        {
            return t.template;
        }

        private void CompileTemplate(string template)
        {

            var chunks = new Regex(@"(\[[A-Z]+\])").Split(template);
            foreach (var chunk in chunks)
            {
                if (chunk.Trim().Length == 0)
                {
                    continue;
                }

                var token = new Regex(@"(\[[A-Z]+\])").IsMatch(chunk) ? GetParser(chunk) : new StaticToken(chunk);

                this.grammar.AddLast(token);
            }
        }

        private static IGrammar GetParser(string tokenName)
        {
            switch (tokenName)
            {
                case "[AMT]":
                    return new AmountToken();
                case "[UNIT]":
                    return new UnitToken();
                case "[ING]":
                    return new IngredientToken();
                case "[FORM]":
                    return new FormToken();
                case "[PREP]":
                    return new PrepToken();
                case "[ANOMALY]":
                    return new AnomalousToken();
            }

            throw new UnknownTokenException(tokenName);
        }

        public override string ToString()
        {
            return this.template;
        }
    }
}