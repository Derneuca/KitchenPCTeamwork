namespace KitchenPC.Fluent.Menus
{
    using KitchenPC.Context;
    using KitchenPC.Menus;

    /// <summary>Provides the ability to fluently express menu related actions, such as creating, updating, or removing menus.</summary>
    public class MenuAction
    {
        private readonly IKPCContext context;

        public MenuAction(IKPCContext context)
        {
            this.context = context;
        }

        public MenuLoader LoadAll
        {
            get
            {
                return new MenuLoader(this.context);
            }
        }

        public MenuCreator Create
        {
            get
            {
                return new MenuCreator(this.context);
            }
        }

        public MenuLoader Load(Menu menu)
        {
            return new MenuLoader(this.context, menu);
        }

        public MenuUpdater Update(Menu menu)
        {
            return new MenuUpdater(this.context, menu);
        }

        public MenuDeleter Delete(Menu menu)
        {
            return new MenuDeleter(this.context, menu);
        }
    }
}