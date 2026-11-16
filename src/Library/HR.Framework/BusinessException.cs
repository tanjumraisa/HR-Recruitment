namespace HR.Framework
{
    // Thrown by the Facade layer when a business rule is broken.
    // The UI shows the message to the user.
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}
