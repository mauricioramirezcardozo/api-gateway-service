namespace WebApi.Application.Exceptions;

public class ApiNotFoundException : ApplicationException
{
    public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }

    public ApiNotFoundException(string errorCode, string message)
        : base($"Error en la solicitud {errorCode}", message)
    {
        ErrorsDictionary = new Dictionary<string, string[]>()
        {
            {
                errorCode, new string[] { message }
            }
        };
    }
}
