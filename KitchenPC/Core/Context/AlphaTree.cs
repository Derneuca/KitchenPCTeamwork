namespace KitchenPC.Context
{
    public class AlphaTree
    {
        public AlphaTree()
        {
            this.Head = new Node();
        }

        public Node Head { get; set; }

        public class Node
        {
            private readonly Node[] nodes;

            public Node()
            {
                this.nodes = new Node[26];
            }

            public ConnectorVertex Connections { get; set; }

            public Node AddLink(char c)
            {
                int index = c - 97;
                this.nodes[index] = new Node();
                var result = this.nodes[index];
                return result;
            }

            public bool HasLink(char c)
            {
                int index = c - 97;
                bool result = this.nodes[index] != null;
                return result;
            }

            public Node GetLink(char c)
            {
                int index = c - 97;
                var result = this.nodes[index];
                return result;
            }

            public void AddConnection(IngredientNode node)
            {
                if (this.Connections == null)
                {
                    this.Connections = new ConnectorVertex();
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