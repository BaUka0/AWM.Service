namespace AWM.Service.WebAPI.Common.Contracts.Responses.Auth;

public record RoleAccessResponse(
    int Id,
    string Code,
    string Name,
    int UsersCount = 0
);
