namespace KitchenPC.Fluent.Modeler
{
    using KitchenPC.Context.Interfaces;
    using KitchenPC.Modeler;
    using IngredientUsage = KitchenPC.Ingredients.IngredientUsage;

    public class ModelingSessionAction
    {
        private readonly ModelingSession session;
        private int recipes;
        private byte scale;

        public ModelingSessionAction(ModelingSession session)
        {
            this.session = session;
            this.recipes = 5;
            this.scale = 2;
        }

        public ModelingSessionAction(IKPCContext context, IUserProfile profile)
        {
            this.session = context.CreateModelingSession(profile);
        }

        public ModelingSessionAction NumRecipes(int recipes)
        {
            this.recipes = recipes;
            return this;
        }

        public ModelingSessionAction Scale(byte scale)
        {
            this.scale = scale;
            return this;
        }

        public Model Generate()
        {
            var result = this.session.Generate(this.recipes, this.scale);
            return result;
        }

        public CompiledModel Compile()
        {
            var model = this.Generate();
            var result = this.session.Compile(model);
            return result;
        }
    }
}
