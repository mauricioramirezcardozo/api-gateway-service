using WebApi.Application.Enums;

namespace WebApi.Application.Contracts.Dto.Response;

public class ErrorResponseApplication
{
    public ErrorType ErrorType { get; set; }
    public string ErrorDescription { get; set; } = string.Empty;
}