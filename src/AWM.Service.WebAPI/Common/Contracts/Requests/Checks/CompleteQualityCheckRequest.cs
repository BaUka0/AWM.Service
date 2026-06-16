namespace AWM.Service.WebAPI.Common.Contracts.Requests.Checks;

public record CompleteQualityCheckRequest(
    bool IsPassed,
    decimal? ResultValue,
    string? Comment,
    long? AttachmentId);
