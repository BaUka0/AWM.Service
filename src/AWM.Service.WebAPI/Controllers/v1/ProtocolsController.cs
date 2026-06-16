using AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;
using AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;
using AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateAdmittedStudentsList;
using AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;
using AWM.Service.Application.Features.Defense.Schedules.Queries.GenerateScheduleReport;
using AWM.Service.Application.Features.Workflow.Works.Commands.NotifyUnreadyStudents;
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
    [RequireAccess("DEFENSE.PROTOCOL", "Create")]
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
    [RequireAccess("DEFENSE.PROTOCOL", "Update")]
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
    [RequireAccess("DEFENSE.PROTOCOL", "Read")]
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

    /// <summary>
    /// Downloads the PDF list of students admitted to final defense.
    /// </summary>
    [HttpGet("admitted-list")]
    [RequireAccess("DEFENSE.PROTOCOL", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAdmittedStudentsList(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        CancellationToken cancellationToken)
    {
        var query = new GenerateAdmittedStudentsListQuery(orgUnitId, semesterId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return File(result.Value, "application/pdf", $"admitted_students_{orgUnitId}_{semesterId}.pdf");
    }

    /// <summary>
    /// Downloads the PDF defense schedule for a commission.
    /// </summary>
    [HttpGet("schedule-report")]
    [RequireAccess("DEFENSE.PROTOCOL", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleReport(
        [FromQuery] int commissionId,
        CancellationToken cancellationToken)
    {
        var query = new GenerateScheduleReportQuery(commissionId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return File(result.Value, "application/pdf", $"schedule_commission_{commissionId}.pdf");
    }

    /// <summary>
    /// Sends notifications to all students who are not admitted to defense in a department/semester.
    /// </summary>
    [HttpPost("notify-unready")]
    [RequireAccess("DEFENSE.PROTOCOL", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> NotifyUnreadyStudents(
        [FromBody] NotifyUnreadyStudentsRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<NotifyUnreadyStudentsCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }
}
