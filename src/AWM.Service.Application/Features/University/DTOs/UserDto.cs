namespace AWM.Service.Application.Features.University.DTOs;

public record UserDto(int Id, string FullName, string Email, string? Iin, string? Phone, bool IsActive = true, DateTime? CreatedAt = null, IReadOnlyList<string>? Roles = null);
