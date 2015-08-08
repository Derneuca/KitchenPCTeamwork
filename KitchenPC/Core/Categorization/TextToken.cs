namespace KitchenPC.Categorization
{
    internal class TextToken : IToken
    {
        private readonly string text;

        public TextToken(string text)
        {
            this.text = text.Trim().ToLower();
        }

        public override bool Equals(object obj)
        {
            var token = obj as TextToken;
            bool result = token != null && token.text.Equals(this.text);
            return result;
        }

        public override int GetHashCode()
        {
            return this.text.GetHashCode();
        }

        public override string ToString()
        {
            return this.text;
        }
    }
}