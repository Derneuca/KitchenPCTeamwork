namespace KitchenPC.Fluent.Queue
{
    using KitchenPC.Context.Interfaces;

    /// <summary>Provides the ability to fluently express recipe queue related actions, such as loading queue and enqueueing/dequeueing recipes.</summary>
    public class QueueAction
    {
        private readonly IKPCContext context;

        public QueueAction(IKPCContext context)
        {
            this.context = context;
        }

        public QueueLoader Load
        {
            get
            {
                return new QueueLoader(this.context);
            }
        }

        public RecipeEnqueuer Enqueue
        {
            get
            {
                return new RecipeEnqueuer(this.context);
            }
        }

        public RecipeDequeuer Dequeue
        {
            get
            {
                return new RecipeDequeuer(this.context);
            }
        }
    }
}
