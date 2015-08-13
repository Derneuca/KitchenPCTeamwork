namespace KitchenPC.NLP
{
    public class AlphaTree<T>
    {
        public AlphaTree()
        {
            this.Head = new Node();
        }

        public Node Head { get; private set; }

        public class Node
        {
            public Node()
            {
                this.Nodes = new Node[94];
            }

            public Node[] Nodes { get; set; }

            public ConnectorVertex<T> Connections { get; private set; }

            public Node AddLink(char c)
            {
                var index = c - 32;
                return this.Nodes[index] = new Node();
            }

            public bool HasLink(char c)
            {
                var index = c - 32;
                return this.Nodes[index] != null;
            }

            public Node GetLink(char c)
            {
                var index = c - 32;
                return this.Nodes[index];
            }

            public void AddConnection(T node)
            {
                if (this.Connections == null)
                {
                    this.Connections = new ConnectorVertex<T>();
                }
                else
                {
                    if (this.Connections.HasConnection(node))
                    {
                        return;
                    }
                }

                this.Connections.AddConnection(node);
            }
        }
    }
}