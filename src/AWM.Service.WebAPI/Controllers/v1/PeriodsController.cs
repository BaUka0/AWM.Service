namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Common.Commands.Periods.CreatePeriod;
using AWM.Service.Application.Features.Common.Commands.Periods.UpdatePeriod;
using AWM.Service.Application.Features.Common.Queries.Periods.GetActivePeriod;
using AWM.Service.Application.Features.Common.Queries.Periods.GetPeriodsByDepartment;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Common;
using AWM.Service.WebAPI.Common.Contracts.Responses.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Periods (workflow stage time constraints).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/departments/{departmentId}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class PeriodsController : BaseController
{
    private readonly ISender _sender;

    public PeriodsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all periods for a department in an academic year.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic Year ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of periods</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Periods_View)]
    [ProducesResponseType(typeof(IReadOnlyList<PeriodResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(int departmentId, [FromQuery] int academicYearId, CancellationToken cancellationToken = default)
    {
        var query = new GetPeriodsByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value
            .Select(dto => new PeriodResponse
            {
                Id = dto.Id,
                DepartmentId = dto.DepartmentId,
                AcademicYearId = dto.AcademicYearId,
                WorkflowStage = dto.WorkflowStage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                IsCurrentlyOpen = dto.IsCurrentlyOpen
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Get the active period for a specific workflow stage.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic Year ID</param>
    /// <param name="stage">Workflow stage</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active period or null</returns>
    [HttpGet("active")]
    [RequireDepartmentPermission(Permission.Periods_View)]
    [ProducesResponseType(typeof(PeriodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActive(
        int departmentId,
        [FromQuery] int academicYearId,
        [FromQuery] WorkflowStage stage,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActivePeriodQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId,
            WorkflowStage = stage
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

        var response = new PeriodResponse
        {
            Id = dto.Id,
            DepartmentId = dto.DepartmentId,
            AcademicYearId = dto.AcademicYearId,
            WorkflowStage = dto.WorkflowStage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            IsCurrentlyOpen = dto.IsCurrentlyOpen
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new period.
    /// </summary>
    /// <param name="departmentId">Department ID (from route)</param>
    /// <param name="request">Create period request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created period ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Periods_Manage)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(int departmentId, [FromBody] CreatePeriodRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreatePeriodCommand
        {
            DepartmentId = departmentId,
            AcademicYearId = request.AcademicYearId,
            WorkflowStage = request.WorkflowStage,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetAll), new { departmentId, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing period.
    /// </summary>
    /// <param name="departmentId">Department ID (from route)</param>
    /// <param name="periodId">Period ID</param>
    /// <param name="request">Update period request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{periodId}")]
    [RequireDepartmentPermission(Permission.Periods_Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int departmentId, int periodId, [FromBody] UpdatePeriodRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdatePeriodCommand
        {
            PeriodId = periodId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
