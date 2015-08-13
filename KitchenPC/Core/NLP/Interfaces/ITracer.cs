namespace KitchenPC.NLP
{
    using Enums;

    public interface ITracer
    {
        void Trace(TraceLevel level, string message, params object[] args);
    }
}