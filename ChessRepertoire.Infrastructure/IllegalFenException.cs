namespace ChessRepertoire.Infrastructure;

public class IllegalFenException : Exception
{
    public IllegalFenException(string message) : base(message)
    {
    }

    public IllegalFenException(string message, Exception innerException) : base(message, innerException) { }
}
