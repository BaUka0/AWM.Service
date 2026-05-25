using AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.CreateCriteria;
using AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.DeleteCriteria;
using AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.UpdateCriteria;
using AWM.Service.Application.Features.Defense.EvaluationCriteria.Queries.GetCriteria;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing evaluation criteria for defenses.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/evaluation-criteria")]
[ApiController]
[Authorize]
public sealed class EvaluationCriteriaController : BaseController
{
    private readonly ISender _sender;

    public EvaluationCriteriaController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets evaluation criteria based on work type and organizational context.
    /// Supports speciality override with fallback to department or university-wide criteria.
    /// </summary>
    [HttpGet]
    [RequireAccess("SYSTEM.STAGE", "Read")]
    public async Task<IActionResult> GetCriteria(
        [FromQuery] int workTypeId,
        [FromQuery] int? orgUnitId,
        [FromQuery] int? specialityId,
        CancellationToken cancellationToken)
    {
        var query = new GetCriteriaQuery(workTypeId, orgUnitId, specialityId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new evaluation criteria.
    /// </summary>
    [HttpPost]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> CreateCriteria(
        [FromBody] CreateCriteriaRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateCriteriaCommand>();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates an existing evaluation criteria.
    /// </summary>
    [HttpPut("{id}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> UpdateCriteria(
        int id,
        [FromBody] UpdateCriteriaRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateCriteriaCommand>() with { Id = id };
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Deletes (soft-delete) an evaluation criteria.
    /// </summary>
    [HttpDelete("{id}")]
    [RequireAccess("SYSTEM.STAGE", "Update")]
    public async Task<IActionResult> DeleteCriteria(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCriteriaCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }
}
