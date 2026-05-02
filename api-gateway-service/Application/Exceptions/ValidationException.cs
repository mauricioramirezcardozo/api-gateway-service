namespace WebApi.Application.Exceptions;

public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
    : ApplicationException("Error de validación", "Ocurrierón uno o mas errores de validacion.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errorsDictionary;
}
