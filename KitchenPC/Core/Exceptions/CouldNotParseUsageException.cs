namespace KitchenPC.Exceptions
{
    using KitchenPC.NLP;

    public class CouldNotParseUsageException : KPCException
    {
        public CouldNotParseUsageException(Result result, string usage)
        {
            this.Result = result;
            this.Usage = usage;
        }

        public Result Result { get; private set; }

        public string Usage { get; private set; }
    }
}
