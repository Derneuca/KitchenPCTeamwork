namespace KitchenPC.NLP
{
    using Enums;
    using log4net;

    /// <summary>Implementation of ITracer that uses Log4Net</summary>
    public class DefaultTracer : ITracer
    {
        private readonly ILog log;

        public DefaultTracer()
        {
            this.log = LogManager.GetLogger(typeof(Parser));
            this.log.Info("Initialized logger for new NLP parser.");
        }

        public void Trace(TraceLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case TraceLevel.Debug:
                    this.log.DebugFormat(message, args);
                    break;
                case TraceLevel.Error:
                    this.log.ErrorFormat(message, args);
                    break;
                case TraceLevel.Fatal:
                    this.log.FatalFormat(message, args);
                    break;
                case TraceLevel.Info:
                    this.log.InfoFormat(message, args);
                    break;
                case TraceLevel.Warn:
                    this.log.WarnFormat(message, args);
                    break;
            }
        }
    }
}