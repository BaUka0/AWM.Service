namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

/// <summary>
/// Request to complete the topic reconciliation stage for a department/semester.
/// </summary>
public record CompleteReconciliationRequest(int OrgUnitId, int SemesterId, int? SpecialityId = null);
