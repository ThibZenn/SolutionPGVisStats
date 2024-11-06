namespace VisStatsBL.Exeptions
{
    public class DomeinException : Exception
    {
        public DomeinException(string? message) : base(message)
        {
        }

        public DomeinException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}