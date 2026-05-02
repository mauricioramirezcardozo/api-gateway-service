using System.Net.Mime;
using System.Text.Json;
using WebApi.Application.Contracts.Dto.Response;
using WebApi.Application.Enums;
using WebApi.Application.Exceptions;

namespace WebApi.Middlewares;

internal sealed class ApplicationMiddleware(ILogger<ApplicationMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ApiBadRequestException ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleException(context, ex);
        }
        catch (BadHttpRequestException ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleException(context, ex);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleException(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var errors = GetErrors(exception);

        var errorData = errors.AsQueryable().First();
        var response = new ErrorResponseApplication
        {
            ErrorType = ErrorType.failed_validation,
            ErrorDescription = string.Join("|", errorData.Value)
        };

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ValidationException or BadHttpRequestException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
    {
        var exceptionMessage = exception.Message;

        if (exception.Message.Contains("bind") || exception.Message.Contains("parameter"))
        {
            exceptionMessage = "Parameters are missing from the submission";
        }

        var innerException = exception.InnerException?.Message;

        if (innerException == null)
            return exception switch
            {
                ApiBadRequestException badRequestEx => badRequestEx.ErrorsDictionary,
                ApiNotFoundException notFoundEx => notFoundEx.ErrorsDictionary,
                BadHttpRequestException => DefError("400", exceptionMessage),
                ApiException apiEx => apiEx.ErrorsDictionary,
                _ => exception.InnerException?.Source == "System.Text.Json"
                    ? DefError("500", innerException ?? "Internal Error")
                    : DefError("500", exception.Message)
            };
        
        var index = innerException.IndexOf('|');
        if (index != -1) innerException = innerException[..index];

        var innerExceptionMessage = exception.InnerException?.InnerException?.Message;
        innerException = exception.InnerException?.InnerException != null
            ? innerException + "| " + innerExceptionMessage
            : innerException.TrimEnd() + ".";

        return exception switch
        {
            ApiBadRequestException badRequestEx => badRequestEx.ErrorsDictionary,
            ApiNotFoundException notFoundEx => notFoundEx.ErrorsDictionary,
            BadHttpRequestException => DefError("400", exceptionMessage),
            ApiException apiEx => apiEx.ErrorsDictionary,
            _ => exception.InnerException?.Source == "System.Text.Json"
                ? DefError("500", innerException)
                : DefError("500", exception.Message)
        };

        static IReadOnlyDictionary<string, string[]> DefError(string code, string des)
            => new Dictionary<string, string[]> { { code, [des] } };
    }
}