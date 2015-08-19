namespace KitchenPC.Context
{
    using System.Collections.Generic;
    using System.Linq;

    public class ConnectorVertex
    {
        private readonly List<IngredientNode> connections;

        public ConnectorVertex()
        {
            this.connections = new List<IngredientNode>();
        }

        public IEnumerable<IngredientNode> Connections
        {
            get
            {
                return this.connections.AsEnumerable();
            }
        }

        public void AddConnection(IngredientNode node)
        {
            this.connections.Add(node);
        }

        public bool HasConnection(IngredientNode node)
        {
            return this.connections.Contains<IngredientNode>(node);
        }
    }
}