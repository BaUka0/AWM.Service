namespace AWM.Service.WebAPI.Common.Contracts.Requests;

/// <summary>
/// Request model for login endpoint.
/// </summary>
public record LoginRequest(string Login, string Password);
