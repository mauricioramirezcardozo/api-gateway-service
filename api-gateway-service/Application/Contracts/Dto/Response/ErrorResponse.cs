namespace WebApi.Application.Contracts.Dto.Response;

public class ErrorResponse
{
    public string StatusCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
