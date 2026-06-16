namespace AWM.Service.WebAPI.Common.Contracts.Requests.Checks;

public record SaveCheckConfigurationRequest(
    int OrgUnitId,
    int CheckTypeId,
    int? SpecialityId,
    decimal? MinimumPassValue,
    bool IsActive);
