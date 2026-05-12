namespace BdsAdmin.API.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message, object? errors = null)
        : base(message, errors)
    {
    }
}
