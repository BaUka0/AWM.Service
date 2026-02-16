using AWM.Service.Application.Features.Common.Commands.Periods.CreatePeriod;
using AWM.Service.Application.Features.Common.Commands.Periods.UpdatePeriod;
using AWM.Service.Application.Features.Common.Queries.Periods.GetActivePeriod;
using AWM.Service.Application.Features.Common.Queries.Periods.GetPeriodsByDepartment;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Periods (workflow stage time constraints).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/departments/{departmentId}/[controller]")]
[ApiController]
[Produces("application/json")]
public class PeriodsController : BaseController
{
    private readonly ISender _sender;

    public PeriodsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get all periods for a department in an academic year.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic Year ID</param>
    /// <returns>List of periods</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Common.DTOs.PeriodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int departmentId, [FromQuery] int academicYearId)
    {
        var query = new GetPeriodsByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get the active period for a specific workflow stage.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic Year ID</param>
    /// <param name="stage">Workflow stage</param>
    /// <returns>Active period or null</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(Application.Features.Common.DTOs.PeriodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive(int departmentId, [FromQuery] int academicYearId, [FromQuery] WorkflowStage stage)
    {
        var query = new GetActivePeriodQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            WorkflowStage = stage
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new period.
    /// </summary>
    /// <param name="departmentId">Department ID (from route)</param>
    /// <param name="request">Create period request</param>
    /// <returns>Created period ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Periods_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(int departmentId, [FromBody] CreatePeriodRequest request)
    {
        var command = new CreatePeriodCommand
        {
            DepartmentId = departmentId,
            AcademicYearId = request.AcademicYearId,
            WorkflowStage = request.WorkflowStage,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetAll), new { departmentId, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing period.
    /// </summary>
    /// <param name="departmentId">Department ID (from route)</param>
    /// <param name="periodId">Period ID</param>
    /// <param name="request">Update period request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{periodId}")]
    [RequireDepartmentPermission(Permission.Periods_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int departmentId, int periodId, [FromBody] UpdatePeriodRequest request)
    {
        var command = new UpdatePeriodCommand
        {
            PeriodId = periodId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
