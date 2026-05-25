using Microsoft.AspNetCore.Http;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Attachments;

public record UploadExpertDocumentRequest(
    int AttachmentTypeId,
    IFormFile File);
