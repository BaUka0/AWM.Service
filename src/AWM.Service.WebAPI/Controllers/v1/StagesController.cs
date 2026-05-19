namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Common.Stages.Commands.ApproveDefenseStages;
using AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;
using AWM.Service.Application.Features.Common.Stages.Commands.CreateStage;
using AWM.Service.Application.Features.Common.Stages.Commands.UpdateStage;
using AWM.Service.Application.Features.Common.Stages.DTOs;
using AWM.Service.Application.Features.Common.Stages.Queries.GetActiveStage;
using AWM.Service.Application.Features.Common.Stages.Queries.GetStagesByDepartment;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Common;
using AWM.Service.WebAPI.Common.Contracts.Responses.Common;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Stages (workflow stage time constraints).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/departments/{departmentId}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class StagesController : BaseController
{
    private readonly ISender _sender;

    public StagesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all stages for a department in a semester.
    /// </summary>
    [HttpGet]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<StageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int departmentId, [FromQuery] int semesterId, CancellationToken cancellationToken = default)
    {
        var query = new GetStagesByDepartmentQuery
        {
            OrgUnitId = departmentId,
            SemesterId = semesterId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value.Adapt<IReadOnlyList<StageResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get the active stage for a specific workflow stage.
    /// </summary>
    [HttpGet("active")]
    [RequireAccess("Organization", "Read")]
    [ProducesResponseType(typeof(StageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive(
        int departmentId,
        [FromQuery] int semesterId,
        [FromQuery] int? workflowStageId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveStageQuery
        {
            OrgUnitId = departmentId,
            SemesterId = semesterId,
            WorkflowStageId = workflowStageId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var dto = result.Value;

        if (dto is null)
        {
            return NotFound();
        }

        var response = dto.Adapt<StageResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new stage.
    /// </summary>
    [HttpPost]
    [RequireAccess("Organization", "Create")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(int departmentId, [FromBody] CreateStageRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateStageCommand>() with { OrgUnitId = departmentId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetAll), new { departmentId, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing stage.
    /// </summary>
    [HttpPut("{stageId}")]
    [RequireAccess("Organization", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int departmentId, int stageId, [FromBody] UpdateStageRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<UpdateStageCommand>() with { StageId = stageId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Bulk approve initial stages (DirectionSubmission, TopicCreation, TopicSelection).
    /// </summary>
    [HttpPost("approve-initial")]
    [RequireAccess("Organization", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveInitialStages(
        [FromRoute] int departmentId,
        [FromQuery] int semesterId,
        [FromBody] ApproveInitialStagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var stages = request.Stages.Adapt<IReadOnlyList<StageSettingsDto>>();

        var command = new ApproveInitialStagesCommand(
            departmentId,
            semesterId,
            stages);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Bulk approve defense stages (PreDefense1, PreDefense2, PreDefense3, FinalDefense).
    /// </summary>
    [HttpPost("approve-defense")]
    [RequireAccess("Organization", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveDefenseStages(
        [FromRoute] int departmentId,
        [FromQuery] int semesterId,
        [FromBody] ApproveDefenseStagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var stages = request.Stages.Adapt<IReadOnlyList<StageSettingsDto>>();

        var command = new ApproveDefenseStagesCommand(
            departmentId,
            semesterId,
            stages);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
