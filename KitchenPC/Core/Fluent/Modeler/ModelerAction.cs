namespace KitchenPC.Fluent.Modeler
{
    using System;

    using KitchenPC.Context;
    using KitchenPC.Modeler;

    using IngredientUsage = KitchenPC.Ingredients.IngredientUsage;

    /// <summary>Provides the ability to fluently express modeler related actions, such as generating or compiling a model.</summary>
    public class ModelerAction
    {
        private readonly IKPCContext context;

        public ModelerAction(IKPCContext context)
        {
            this.context = context;
        }

        public ModelingSessionAction WithAnonymous
        {
            get
            {
                var result = new ModelingSessionAction(this.context, UserProfile.Anonymous);
                return result;
            }
        }

        public ModelingSessionAction WithSession(ModelingSession session)
        {
            var result = new ModelingSessionAction(session);
            return result;
        }

        public ModelingSessionAction WithProfile(IUserProfile profile)
        {
            var result = new ModelingSessionAction(context, profile);
            return result;
        }

        public ModelingSessionAction WithProfile(Func<ProfileCreator, ProfileCreator> profileCreator)
        {
            var creator = profileCreator(new ProfileCreator());
            var result = new ModelingSessionAction(this.context, creator.Profile);
            return result;
        }
    }
}
