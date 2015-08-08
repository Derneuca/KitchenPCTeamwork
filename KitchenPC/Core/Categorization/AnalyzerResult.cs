namespace KitchenPC.Categorization
{
    using System;

    public class AnalyzerResult
    {
        public AnalyzerResult(Category firstPlace, Category secondPlace)
        {
            this.FirstPlace = firstPlace;
            this.SecondPlace = secondPlace;
        }

        public Category FirstPlace { get; private set; }

        public Category SecondPlace { get; private set; }

        public override string ToString()
        {
            return this.SecondPlace == Category.None ?
                this.FirstPlace.ToString() :
                string.Format("{0}/{1}", this.FirstPlace, this.SecondPlace);
        }
    }
}