namespace AWM.Service.WebAPI.Common.Contracts.Requests;

public record ConfirmEmployeesRequest(
    int SemesterId,
    int? SpecialityId = null
);
