namespace KitchenPC.Context
{
    using System;

    public class IngredientNode
    {
        public IngredientNode(Guid id, string name, int popularity)
        {
            this.Id = id;
            this.IngredientName = name;
            this.Popularity = popularity;
        }

        public Guid Id { get; set; }

        public string IngredientName { get; set; }

        public int Popularity { get; set; }
    }
}