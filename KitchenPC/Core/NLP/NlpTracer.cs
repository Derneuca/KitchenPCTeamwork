namespace KitchenPC.NLP
{
    using Enums;

    public static class NlpTracer
    {
        private static ITracer currentTracer;

        public static void SetTracer(ITracer tracer)
        {
            currentTracer = tracer;
        }

        public static void Trace(TraceLevel level, string message, params object[] args)
        {
            if (currentTracer == null)
            {
                return;
            }

            currentTracer.Trace(level, message, args);
        }

        public static void ConditionalTrace(
            bool condition,
            TraceLevel level, 
            string message, 
            params object[] args)
        {
            if (condition)
            {
                Trace(level, message, args);
            }
        }
    }
}