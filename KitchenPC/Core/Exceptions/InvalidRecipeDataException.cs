namespace KitchenPC.Exceptions
{
    public class InvalidRecipeDataException : KPCException
    {
        public InvalidRecipeDataException(string message)
            : base(message)
        {
        }
    }
}
