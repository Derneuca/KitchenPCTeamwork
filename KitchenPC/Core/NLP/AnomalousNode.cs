namespace KitchenPC.NLP
{
    public class AnomalousNode
    {
        public AnomalousNode(string name, AnomalousIngredientNode ingredient)
        {
            this.Name = name;
            this.Ingredient = ingredient;
        }

        public string Name { get; set; }
        
        public AnomalousIngredientNode Ingredient { get; set; }
    }
}