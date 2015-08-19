namespace KitchenPC.Modeler
{
    using System;

    using KitchenPC.Context.Interfaces;

    public class ModelerProxy
    {
        private readonly IKPCContext context;
        private DBSnapshot dataBase;

        public ModelerProxy(IKPCContext context)
        {
            this.context = context;
        }

        public void LoadSnapshot()
        {
            this.dataBase = new DBSnapshot(this.context);
        }

        public ModelingSession CreateSession(IUserProfile profile)
        {
            if (this.dataBase == null)
            {
                throw new Exception("ModelerProxy has not been initialized.");
            }
            
            var session = new ModelingSession(this.context, this.dataBase, profile);
            return session;
        }

        public RecipeNode FindRecipe(Guid id)
        {
            if (this.dataBase == null)
            {
                throw new Exception("ModelerProxy has not been initialized.");
            }

            var recipeNode = this.dataBase.FindRecipe(id);
            return recipeNode;
        }

        public IngredientNode FindIngredient(Guid id)
        {
            if (this.dataBase == null)
            {
                throw new Exception("ModelerProxy has not been initialized.");
            }

            var ingredientNode = this.dataBase.FindIngredient(id);
            return ingredientNode;
        }
    }
}