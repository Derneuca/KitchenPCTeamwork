namespace KitchenPC.Categorization
{
    using System;

    public class AnalyzerResult
    {
        public Category FirstPlace { get; private set; }

        public Category SecondPlace { get; private set; }

        public AnalyzerResult(Category first, Category second)
        {
            this.FirstPlace = first;
            this.SecondPlace = second;
        }

        public override string ToString()
        {
            return this.SecondPlace == Category.None ? this.FirstPlace.ToString()
               : String.Format("{0}/{1}", this.FirstPlace, this.SecondPlace);
        }
    }
}