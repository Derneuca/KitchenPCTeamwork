namespace KitchenPC.Categorization
{
    using KitchenPC.Categorization.Enums;
    using KitchenPC.Categorization.Interfaces;

    internal class TimeToken : IToken
    {
        private readonly Classification classification;

        public TimeToken(int minutes)
        {
            if (minutes < 10)
            {
                this.classification = Classification.Quick;
            }
            else if (minutes < 30)
            {
                this.classification = Classification.Medium;
            }
            else if (minutes <= 60)
            {
                this.classification = Classification.Long;
            }
            else
            {
                this.classification = Classification.SuperLong;
            }
        }

        public override bool Equals(object obj)
        {
            var token = obj as TimeToken;
            bool result = token != null && token.classification.Equals(this.classification);
            return result;
        }

        public override int GetHashCode()
        {
            return this.classification.GetHashCode();
        }

        public override string ToString()
        {
            return this.classification.ToString();
        }
    }
}