namespace AWM.Service.WebAPI.Common.Contracts.Requests.Users;

/// <summary>
/// Request to change user activation status.
/// </summary>
public sealed record ToggleUserStatusRequest(bool IsActive);
