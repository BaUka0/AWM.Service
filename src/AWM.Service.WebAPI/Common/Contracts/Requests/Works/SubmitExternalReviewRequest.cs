using Microsoft.AspNetCore.Http;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Works;

/// <summary>
/// Request contract for uploading external reviewer's feedback.
/// </summary>
public sealed record SubmitExternalReviewRequest(
    IFormFile File);
