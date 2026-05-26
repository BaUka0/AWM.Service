namespace AWM.Service.WebAPI.Common.Contracts.Requests;

public record ConfirmSupervisorsRequest(
    int SemesterId,
    int? SpecialityId = null
);
