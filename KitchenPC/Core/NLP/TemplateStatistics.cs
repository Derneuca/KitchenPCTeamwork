namespace KitchenPC.NLP
{
    using System.Collections;
    using System.Collections.Generic;

    public class TemplateStatistics : IEnumerable<TemplateStatistics.TemplateUsage>
    {
        private readonly Dictionary<Template, int> stats;

        public TemplateStatistics()
        {
            this.stats = new Dictionary<Template, int>();
        }

        public int this[Template t]
        {
            get
            {
                return this.stats[t];
            }
            set
            {
                this.stats[t] = value;
            }
        }

        public void RecordTemplate(Template template)
        {
            this.stats[template] = 0;
        }

        public IEnumerator<TemplateUsage> GetEnumerator()
        {
            var e = this.stats.GetEnumerator();
            while (e.MoveNext())
            {
                yield return new TemplateUsage() { Template = e.Current.Key.ToString(), Matches = e.Current.Value };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var e = this.stats.GetEnumerator();
            while (e.MoveNext())
            {
                yield return new TemplateUsage
                                 {
                                     Template = e.Current.Key.ToString(), Matches = e.Current.Value
                                 };
            }
        }

        public struct TemplateUsage
        {
            public string Template { get; set; }

            public int Matches { get; set; }

            public override string ToString()
            {
                return string.Format("{0} ---> {1} matches\n", this.Template, this.Matches);
            }
        }
    }
}