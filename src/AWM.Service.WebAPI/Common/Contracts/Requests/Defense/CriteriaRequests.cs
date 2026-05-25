namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public record CreateCriteriaRequest(
    int WorkTypeId,
    string CriteriaName,
    int MaxScore,
    decimal Weight = 1.0m,
    int? OrgUnitId = null,
    int? SpecialityId = null
);

public record UpdateCriteriaRequest(
    string CriteriaName,
    int MaxScore,
    decimal Weight
);
