namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Attachments.Commands.DeleteAttachment;
using AWM.Service.Application.Features.Thesis.Attachments.Commands.UploadAttachment;
using AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentById;
using AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentsByWork;
using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing file attachments of a student work.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/works/{workId:long}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class AttachmentsController : BaseController
{
    private readonly ISender _sender;
    private readonly IAttachmentService _attachmentService;

    public AttachmentsController(ISender sender, IAttachmentService attachmentService)
    {
        _sender = sender;
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
    }

    /// <summary>
    /// Get all attachments for a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of attachments.</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Attachments_Download)]
    [ProducesResponseType(typeof(IReadOnlyList<AttachmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(long workId, CancellationToken cancellationToken = default)
    {
        var query = new GetAttachmentsByWorkQuery { WorkId = workId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<AttachmentResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific attachment by ID.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Attachment metadata.</returns>
    [HttpGet("{attachmentId:long}")]
    [RequireDepartmentPermission(Permission.Attachments_Download)]
    [ProducesResponseType(typeof(AttachmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long workId, long attachmentId, CancellationToken cancellationToken = default)
    {
        var query = new GetAttachmentByIdQuery { WorkId = workId, AttachmentId = attachmentId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<AttachmentResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Upload a new file attachment to a student work (multipart/form-data).
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="request">Upload request with file and attachment type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of the created attachment.</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Attachments_Upload)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upload(
        long workId,
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UploadAttachmentCommand>() with { WorkId = workId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(
            nameof(GetById),
            new { workId, attachmentId = result.Value, version = "1.0" },
            result.Value);
    }

    /// <summary>
    /// Download a file attachment (returns the raw file stream).
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>File stream with original content type.</returns>
    [HttpGet("{attachmentId:long}/download")]
    [RequireDepartmentPermission(Permission.Attachments_Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download(
        long workId,
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        // Resolve metadata first
        var query = new GetAttachmentByIdQuery { WorkId = workId, AttachmentId = attachmentId };
        var metaResult = await _sender.Send(query, cancellationToken);

        if (metaResult.IsFailed)
            return HandleResultError(metaResult.Error);

        var dto = metaResult.Value;

        try
        {
            var stream = await _attachmentService.GetAsync(dto.FileStoragePath, cancellationToken);

            var contentType = GetContentType(dto.FileName);
            var downloadName = dto.FileName;

            return File(stream, contentType, downloadName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { Code = "404", Message = $"Physical file not found for attachment {attachmentId}." });
        }
    }

    /// <summary>
    /// Delete a file attachment from a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{attachmentId:long}")]
    [RequireDepartmentPermission(Permission.Attachments_Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        long workId,
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAttachmentCommand { WorkId = workId, AttachmentId = attachmentId };
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }
}
