namespace KitchenPC.Ingredients
{
    using System.Collections.Generic;

    public class IngredientFormsCollection
    {
        private readonly List<IngredientForm> forms;

        public IngredientFormsCollection()
            : this(null)
        { 
        }

        public IngredientFormsCollection(IEnumerable<IngredientForm> forms)
        {
            this.forms = new List<IngredientForm>(forms);
        }

        public IngredientForm[] Forms
        {
            get
            {
                return this.forms.ToArray();
            }

            set
            {
                this.forms.Clear();
                this.forms.AddRange(value);
            }
        }

        public void AddForm(IngredientForm form)
        {
            this.forms.Add(form);
        }
    }
}