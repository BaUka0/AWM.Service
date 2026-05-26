using AWM.Service.Application.Features.Workflow.Attachments.Commands.DeleteAttachment;
using AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadAttachment;
using AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadExpertDocument;
using AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadAttachment;
using AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadExpertDocument;
using AWM.Service.Application.Features.Workflow.Attachments.Queries.GetWorkAttachments;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Attachments;
using AWM.Service.WebAPI.Common.Contracts.Responses.Attachments;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing attachments and expert check documents for student works.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/student-works/{workId:long}/attachments")]
[ApiController]
[Authorize]
public sealed class AttachmentsController : BaseController
{
    private readonly ISender _sender;

    public AttachmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets all attachments for a specific student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of attachments.</returns>
    [HttpGet]
    [RequireAccess("THESIS.ATTACHMENT", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<AttachmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkAttachments(long workId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetWorkAttachmentsQuery(workId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<AttachmentResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Uploads a new attachment to a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="request">The file and attachment type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created attachment.</returns>
    [HttpPost]
    [RequireAccess("THESIS.ATTACHMENT", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAttachment(
        long workId,
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        using var stream = request.File.OpenReadStream();
        var command = new UploadAttachmentCommand(
            workId,
            request.AttachmentTypeId,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            stream
        );

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Downloads an attachment for a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file stream.</returns>
    [HttpGet("{attachmentId:long}/download")]
    [RequireAccess("THESIS.ATTACHMENT", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(long workId, long attachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DownloadAttachmentQuery(workId, attachmentId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var dto = result.Value;
        return File(dto.FileStream, dto.ContentType, dto.FileName);
    }

    /// <summary>
    /// Deletes an attachment from a student work.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content status.</returns>
    [HttpDelete("{attachmentId:long}")]
    [RequireAccess("THESIS.ATTACHMENT", "Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(long workId, long attachmentId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteAttachmentCommand(workId, attachmentId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Uploads an expert document linked to a quality check.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="checkId">Quality check ID.</param>
    /// <param name="request">The file and attachment type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created attachment.</returns>
    [HttpPost("quality-checks/{checkId:long}/document")]
    [RequireAccess("THESIS.CHECK", "Update")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadExpertDocument(
        long workId,
        long checkId,
        [FromForm] UploadExpertDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        using var stream = request.File.OpenReadStream();
        var command = new UploadExpertDocumentCommand(
            workId,
            checkId,
            request.AttachmentTypeId,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            stream
        );

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Downloads an expert document linked to a quality check.
    /// </summary>
    /// <param name="workId">Student work ID.</param>
    /// <param name="checkId">Quality check ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file stream.</returns>
    [HttpGet("quality-checks/{checkId:long}/document")]
    [RequireAccess("THESIS.CHECK", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadExpertDocument(long workId, long checkId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DownloadExpertDocumentQuery(workId, checkId), cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var dto = result.Value;
        return File(dto.FileStream, dto.ContentType, dto.FileName);
    }
}
