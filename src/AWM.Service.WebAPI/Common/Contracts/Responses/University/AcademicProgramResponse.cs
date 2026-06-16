namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record AcademicProgramResponse(
    int Id,
    string Code,
    string NameRu,
    string NameKz,
    string NameEn,
    int DepartmentId
);
