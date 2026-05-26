namespace AWM.Service.WebAPI.Common.Contracts.Requests.Reviewers;

public record CreateReviewerRequest(
    string FullName,
    string? Position,
    string? AcademicDegree,
    string? Organization,
    string? Email,
    string? Phone);
