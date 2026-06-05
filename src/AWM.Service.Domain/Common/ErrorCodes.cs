namespace AWM.Service.Domain.Common;

/// <summary>
/// Centralized error codes used across Application handlers.
/// The <see cref="StatusMap"/> is consumed by the WebAPI layer to translate codes to HTTP status codes.
/// </summary>
public static class ErrorCodes
{
    // Auth
    public const string AuthInvalidCredentials = "Auth.InvalidCredentials";
    public const string AuthInvalidRefreshToken = "Auth.InvalidRefreshToken";
    public const string AuthUserNotFound = "Auth.UserNotFound";
    public const string AuthUnauthorized = "Auth.Unauthorized";

    // Register
    public const string RegisterInvalidData = "Register.InvalidData";
    public const string RegisterUserNotFound = "Register.UserNotFound";
    public const string RegisterAccountExists = "Register.AccountAlreadyExists";

    // General
    public const string NotFound = "NotFound";
    public const string Conflict = "Conflict";
    public const string Validation = "Validation";
    public const string Forbidden = "Forbidden";
    public const string RoleNotFound = "RoleNotFound";
    public const string Unauthorized = "Unauthorized";

    /// <summary>
    /// Maps an error code to an HTTP status code integer.
    /// Used by the WebAPI layer in <c>BaseController.HandleResultError</c>.
    /// Unknown codes fall back to 500.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, int> StatusMap =
        new Dictionary<string, int>
        {
            [AuthInvalidCredentials] = 401,
            [AuthInvalidRefreshToken] = 401,
            [AuthUserNotFound] = 404,
            [AuthUnauthorized] = 401,
            [RegisterInvalidData] = 400,
            [RegisterUserNotFound] = 404,
            [RegisterAccountExists] = 409,
            [NotFound] = 404,
            [Conflict] = 409,
            [Validation] = 400,
            [Forbidden] = 403,
            [RoleNotFound] = 404,
            [Unauthorized] = 401,
        };
}
