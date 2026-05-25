using System.IO;

namespace AWM.Service.Application.Features.Workflow.Attachments.DTOs;

public record FileDownloadDto(
    Stream FileStream,
    string FileName,
    string ContentType);
