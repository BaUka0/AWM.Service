using Microsoft.AspNetCore.Http;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Works;

public sealed record SubmitSupervisorReviewRequest(
    IFormFile File,
    string? Comment);
