namespace KitchenPC.Exceptions
{
    using System;

    public class UnknownTokenException : Exception
    {
        public UnknownTokenException(string token)
        {
            this.Token = token;
        }

        public string Token { get; private set; }
    }
}
