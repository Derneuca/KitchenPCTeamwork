namespace KitchenPC.Recipes
{
    public class SearchResults
    {
        public SearchResults()
        {
        }

        public SearchResults(RecipeBrief[] briefs, long total)
        {
            this.Briefs = briefs;
            this.TotalCount = total;
        }

        public RecipeBrief[] Briefs { get; set; }

        public long TotalCount { get; set; }
    }
}