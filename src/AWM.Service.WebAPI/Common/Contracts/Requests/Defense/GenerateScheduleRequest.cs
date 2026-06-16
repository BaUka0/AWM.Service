namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public record GenerateScheduleRequest(
    int CommissionId,
    DateTime StartDate,
    string? Location,
    int SlotDurationMinutes,
    List<long> WorkIds
);
