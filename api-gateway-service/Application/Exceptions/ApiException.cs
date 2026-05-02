namespace WebApi.Application.Exceptions;

internal class ApiException : ApplicationException
{
    public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }

    public ApiException(string errorCode, string message) : base($"Error en el servidor {errorCode}", message)
    {
        ErrorsDictionary = new Dictionary<string, string[]>()
        {
            {
                errorCode, new string[] { message }
            }
        };
    }
}
