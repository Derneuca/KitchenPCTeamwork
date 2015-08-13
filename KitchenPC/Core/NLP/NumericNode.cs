namespace KitchenPC.NLP
{
    public class NumericNode
    {
        public NumericNode(string token, float value)
        {
            this.Token = token;
            this.Value = value;
        }

        public string Token { get; set; }

        public float Value { get; set; }
    }
}