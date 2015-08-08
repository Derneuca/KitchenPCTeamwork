namespace KitchenPC.Categorization
{
    internal class TextToken : IToken
    {
        private readonly string Text;

        public TextToken(string text)
        {
            this.Text = text.Trim().ToLower();
        }

        public override bool Equals(object obj)
        {
            var t1 = obj as TextToken;
            return t1 != null && t1.Text.Equals(this.Text);
        }

        public override int GetHashCode()
        {
            return this.Text.GetHashCode();
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}