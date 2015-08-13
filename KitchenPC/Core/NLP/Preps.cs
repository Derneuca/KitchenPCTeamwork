namespace KitchenPC.NLP
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Preps : IEnumerable<PrepNode>
    {
        private readonly List<PrepNode> notes;

        public Preps()
        {
            this.notes = new List<PrepNode>();
        }

        public bool HasValue
        {
            get
            {
                return this.notes.Count != 0;
            }
        }

        public void Add(PrepNode prepNode)
        {
            this.notes.Add(prepNode);
        }

        public void Remove(PrepNode node)
        {
            this.notes.Remove(node);
        }

        public override string ToString()
        {
            if (this.notes.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("//", this.notes.Select(p => p.Prep).ToArray());
        }

        public IEnumerator<PrepNode> GetEnumerator()
        {
            return this.notes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.notes.GetEnumerator();
        }
    }
}