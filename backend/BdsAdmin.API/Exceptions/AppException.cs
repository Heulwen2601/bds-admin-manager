namespace BdsAdmin.API.Exceptions;

public abstract class AppException : Exception
{
    public object? Errors { get; }

    protected AppException(string message, object? errors = null)
        : base(message)
    {
        Errors = errors;
    }
}
