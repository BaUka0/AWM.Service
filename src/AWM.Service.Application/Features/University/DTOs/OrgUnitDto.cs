namespace AWM.Service.Application.Features.University.DTOs;

public record OrgUnitDto(int Id, string Name, string? Address, int FacultyCount, int? ParentId);
