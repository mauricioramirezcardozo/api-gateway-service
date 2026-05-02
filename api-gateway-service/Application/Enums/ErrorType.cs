namespace WebApi.Application.Enums;

public enum ErrorType
{
    internal_server_error,
    failed_validation,

    error_interno_servidor,
    regla_negocios,
    cuenta_not_found,
    error_validacion
}