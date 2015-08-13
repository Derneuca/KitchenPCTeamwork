namespace KitchenPC.NLP
{
    using System.Collections.Generic;
    using System.Linq;

    public class ConnectorVertex<T>
    {
        private readonly List<T> connections;

        public ConnectorVertex()
        {
            this.connections = new List<T>();
        }

        public void AddConnection(T node)
        {
            this.connections.Add(node);
        }

        public bool HasConnection(T node)
        {
            return this.connections.Contains<T>(node);
        }

        public IEnumerable<T> GetConnections()
        {
            var enumerator = this.connections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}