namespace KitchenPC.NLP
{
    public class UnitNode
    {
        public UnitNode(string name, Units unit)
        {
            this.Name = name.Trim();
            this.Unit = unit;
        }

        public string Name { get; set; }

        public Units Unit { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return this.Name;
            }

            return this.Unit.ToString();
        }
    }
}