namespace Settings.Contracts.Exceptions;
public class AlreadyExistsException : Exception
{
    public AlreadyExistsException() : base()
    {
    }

    public AlreadyExistsException(string message) : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
