namespace AWM.Service.WebAPI.Common.Contracts.Requests;

/// <summary>
/// Request for user registration.
/// </summary>
public record RegisterRequest(
    string Login,
    string Email,
    string Password,
    int UniversityId = 1
);
