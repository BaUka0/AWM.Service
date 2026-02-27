namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.Schedule.Commands.AssignWorkToSlot;
using AWM.Service.Application.Features.Defense.Schedule.Commands.CreateDefenseSchedule;
using AWM.Service.Application.Features.Defense.Schedule.Commands.UpdateDefenseSchedule;
using AWM.Service.Application.Features.Defense.Schedule.DTOs;
using AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSchedule;
using AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSlotById;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using AWM.Service.WebAPI.Common.Contracts.Responses.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing defense schedule (GAK) — CRUD and work assignment.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/defense-schedule")]
[Produces("application/json")]
public class DefenseScheduleController : BaseController
{
    private readonly ISender _sender;

    public DefenseScheduleController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get the defense schedule (all slots) for a GAK commission.
    /// </summary>
    /// <param name="commissionId">Commission ID</param>
    /// <returns>List of schedule slots ordered by defense date</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Defense_View)]
    [ProducesResponseType(typeof(IReadOnlyList<ScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSchedule([FromQuery] int commissionId)
    {
        var query = new GetDefenseScheduleQuery { CommissionId = commissionId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<ScheduleResponse>>();

        return Ok(response);
    }

    /// <summary>
    /// Get a specific defense slot by ID with detailed grade information.
    /// </summary>
    /// <param name="slotId">Schedule (slot) ID</param>
    /// <returns>Detailed slot information with grades</returns>
    [HttpGet("{slotId:long}")]
    [RequireDepartmentPermission(Permission.Defense_View)]
    [ProducesResponseType(typeof(DefenseSlotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSlotById(long slotId)
    {
        var query = new GetDefenseSlotByIdQuery { SlotId = slotId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<DefenseSlotResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Create a new defense schedule slot (Secretary action).
    /// </summary>
    /// <param name="request">Schedule creation details</param>
    /// <returns>Created schedule ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Defense_Schedule)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateDefenseScheduleRequest request)
    {
        var command = request.Adapt<CreateDefenseScheduleCommand>();

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetSlotById), new { slotId = result.Value }, result.Value);
    }

    /// <summary>
    /// Update (reschedule) an existing defense schedule slot (Secretary action).
    /// </summary>
    /// <param name="scheduleId">Schedule ID to update</param>
    /// <param name="request">Update details</param>
    /// <returns>No content on success</returns>
    [HttpPut("{scheduleId:long}")]
    [RequireDepartmentPermission(Permission.Defense_Schedule)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long scheduleId, [FromBody] UpdateDefenseScheduleRequest request)
    {
        var command = request.Adapt<UpdateDefenseScheduleCommand>() with { ScheduleId = scheduleId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Assign a student work to an existing defense schedule slot (Secretary action).
    /// </summary>
    /// <param name="scheduleId">Schedule (slot) ID</param>
    /// <param name="request">Assignment details</param>
    /// <returns>No content on success</returns>
    [HttpPost("{scheduleId:long}/assign")]
    [RequireDepartmentPermission(Permission.Defense_AssignSlot)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignWork(long scheduleId, [FromBody] AssignWorkToSlotRequest request)
    {
        var command = request.Adapt<AssignWorkToSlotCommand>() with { ScheduleId = scheduleId };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
