namespace AWM.Service.WebAPI.Common.Contracts.Requests;

/// <summary>
/// Request contract for refreshing an access token.
/// </summary>
public record RefreshTokenRequest(string RefreshToken);
