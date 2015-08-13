namespace KitchenPC.NLP
{
    using System;

    public class IngredientNode
    {
        private readonly IngredientNode parent;
        private readonly Guid id;
        private readonly DefaultPairings pairings;
        private readonly UnitType convtype;
        private readonly Weight unitweight;

        public IngredientNode(Guid id, string name, UnitType convtype, Weight unitweight, DefaultPairings pairings)
        {
            this.id = id;
            this.pairings = pairings;
            this.convtype = convtype;
            this.unitweight = unitweight;

            this.IngredientName = name;
        }

        public IngredientNode(IngredientNode root, string synonym, string prepnote)
        {
            this.parent = root;
            this.IngredientName = synonym;
            this.PrepNote = prepnote;
        }

        public Guid Id
        {
            get
            {
                return (this.parent == null) ? this.id : this.parent.id;
            }
        }

        public DefaultPairings Pairings
        {
            get
            {
                return (this.parent == null) ? this.pairings : this.parent.pairings;
            }
        }

        public IngredientNode Parent
        {
            get
            {
                return this.parent;
            }
        }

        public string IngredientName { get; set; }

        public string PrepNote { get; set; }

        public UnitType ConversionType
        {
            get
            {
                return (this.parent == null) ? this.convtype : this.parent.convtype;
            }
        }

        public Weight UnitWeight
        {
            get
            {
                return (this.parent == null) ? this.unitweight : this.parent.unitweight;
            }
        }
    }
}