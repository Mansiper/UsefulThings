namespace DataResultExample;

public class RequestException : Exception
{
    public string? Additional { get; }
    public RequestExceptionType DbExceptionType { get; }

    public RequestException() {}
    public RequestException(RequestExceptionType dbExceptionType, string? message = null, string? additional = null) :
        base(message)
    {
        Additional = additional;
        DbExceptionType = dbExceptionType;
    }
}