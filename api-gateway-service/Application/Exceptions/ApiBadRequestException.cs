namespace WebApi.Application.Exceptions;

public class ApiBadRequestException : BadRequestException
{
    public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }

    public ApiBadRequestException(string errorCode, string message) : base(errorCode, message)
    {
        ErrorsDictionary = new Dictionary<string, string[]>
        {
            {
                errorCode, new[] { message }
            }
        };
    }
}
