namespace WebApi.Application.Exceptions;

public abstract class BadRequestException(string errorCode, string message)
    : ApplicationException(errorCode, message)
{
    public string ErrorCode { get; } = errorCode;
}
