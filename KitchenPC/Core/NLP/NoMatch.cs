namespace KitchenPC.NLP
{
    using Enums;

    public class NoMatch : Result
    {
        private readonly MatchResult status;

        public NoMatch(string input, MatchResult status)
            : base(input)
        {
            this.status = status;
        }

        public override MatchResult Status
        {
            get
            {
                return this.status;
            }
        }

        public override string ToString()
        {
            return string.Format("[NoMatch] Error: {0}", this.status);
        }
    }
}