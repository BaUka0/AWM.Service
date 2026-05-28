using AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;
using AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;
using AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing defense protocols (final decisions and session records).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/protocols")]
[ApiController]
[Authorize]
public sealed class ProtocolsController : BaseController
{
    private readonly ISender _sender;

    public ProtocolsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new protocol for a defense session.
    /// </summary>
    [HttpPost]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> CreateProtocol(
        [FromBody] CreateProtocolRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateProtocolCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Finalizes a protocol, locking all grades and decisions.
    /// </summary>
    [HttpPost("{id}/finalize")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> FinalizeProtocol(
        long id,
        [FromBody] FinalizeProtocolRequest? body,
        CancellationToken cancellationToken)
    {
        var command = new FinalizeProtocolCommand(id, body?.IsStudentPresent ?? true);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Downloads the official PDF report for a finalized protocol.
    /// </summary>
    /// <param name="id">Protocol identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A PDF file result.</returns>
    [HttpGet("{id:long}/pdf")]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProtocolPdf(
        long id,
        CancellationToken cancellationToken)
    {
        var query = new GenerateProtocolReportQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return File(result.Value, "application/pdf", $"protocol_{id}.pdf");
    }
}
