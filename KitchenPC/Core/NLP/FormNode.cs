namespace KitchenPC.NLP
{
    public class FormNode
    {
        public FormNode(string name)
        {
            this.FormName = name.Trim();
        }

        public string FormName { get; set; }
    }
}