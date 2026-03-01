namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Response model for successful login.
/// </summary>
public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public string Login { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = Array.Empty<string>();
    public string RefreshToken { get; init; } = string.Empty;
}
