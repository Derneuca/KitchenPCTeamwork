namespace KitchenPC.Exceptions
{
    using System;

    public class KPCException : Exception
    {
        public KPCException()
        {
        }

        public KPCException(string message)
            : base(message)
        {
        }
    }
}
