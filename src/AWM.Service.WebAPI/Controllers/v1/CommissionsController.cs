using AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;
using AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;
using AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;
using AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissions;
using AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;
using AWM.Service.Application.Features.Defense.Commissions.Commands.AutoDistributeStudents;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using AWM.Service.WebAPI.Common.Contracts.Responses;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing commissions (Pre-defense and GAK).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/commissions")]
[ApiController]
[Authorize]
public sealed class CommissionsController : BaseController
{
    private readonly ISender _sender;

    public CommissionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets commissions based on filters.
    /// </summary>
    [HttpGet]
    [RequireAccess("DEFENSE.COMMISSION", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommissions(
        [FromQuery] int orgUnitId,
        [FromQuery] int semesterId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var query = new GetCommissionsQuery(orgUnitId, semesterId, specialityId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a commission by ID.
    /// </summary>
    [HttpGet("{id}")]
    [RequireAccess("DEFENSE.COMMISSION", "Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommissionById(int id, CancellationToken cancellationToken)
    {
        var query = new GetCommissionByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new commission.
    /// </summary>
    [HttpPost]
    [RequireAccess("DEFENSE.COMMISSION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCommission(
        [FromBody] CreateCommissionRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateCommissionCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing commission.
    /// </summary>
    [HttpPut("{id}")]
    [RequireAccess("DEFENSE.COMMISSION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCommission(
        int id,
        [FromBody] UpdateCommissionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommissionCommand(
            id,
            request.Name,
            request.CommissionTypeId,
            request.PreDefenseNumber,
            request.SpecialityId,
            request.ChairmanUserId,
            request.SecretaryUserId,
            request.MemberUserIds);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }

    /// <summary>
    /// Deletes a commission (soft-delete). Fails if students are already assigned.
    /// </summary>
    [HttpDelete("{id}")]
    [RequireAccess("DEFENSE.COMMISSION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCommission(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteCommissionCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }

    /// <summary>
    /// Automatically distributes students to commissions.
    /// </summary>
    [HttpPost("auto-distribute")]
    [RequireAccess("DEFENSE.COMMISSION", "Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AutoDistributeStudents(
        [FromBody] AutoDistributeStudentsRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<AutoDistributeStudentsCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok();
    }
}
