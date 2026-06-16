using Microsoft.AspNetCore.Http;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Attachments;

public record UploadAttachmentRequest(
    int AttachmentTypeId,
    IFormFile File);
