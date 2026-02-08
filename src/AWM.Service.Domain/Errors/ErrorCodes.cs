namespace AWM.Service.Domain.Errors;

/// <summary>
/// Standard error code prefixes used by BaseController to map to HTTP status codes.
/// </summary>
public static class ErrorCodes
{
    public const string NotFound = "NotFound";
    public const string Validation = "Validation";
    public const string BusinessRule = "BusinessRule";
    public const string Conflict = "Conflict";
    public const string Unauthorized = "Unauthorized";
    public const string Forbidden = "Forbidden";
    public const string InternalError = "InternalError";
}
