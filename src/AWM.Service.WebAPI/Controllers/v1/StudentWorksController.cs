namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;
using AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;
using AWM.Service.Application.Features.Thesis.Works.Commands.RemoveParticipant;
using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWork;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorkById;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksByDepartment;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksBySupervisor;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;
using AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing student works.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/works")]
[Produces("application/json")]
public class StudentWorksController : BaseController
{
    private readonly ISender _sender;

    public StudentWorksController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets student works for a specific supervisor.
    /// </summary>
    /// <param name="supervisorId">Supervisor ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of works</returns>
    [HttpGet("supervisor/{supervisorId:int}")]
    [RequireDepartmentPermission(Permission.Works_View)]
    [ProducesResponseType(typeof(IReadOnlyList<StudentWorkResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBySupervisor(
        int supervisorId,
        [FromQuery] int? academicYearId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetStudentWorksBySupervisorQuery { SupervisorId = supervisorId, AcademicYearId = academicYearId ?? 0 },
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value.Adapt<IReadOnlyList<StudentWorkResponse>>())
            : HandleResultError(result.Error);
    }

    /// <summary>
    /// Get a student work by ID with full details.
    /// </summary>
    /// <param name="id">Student work ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed work information including participants</returns>
    [HttpGet("{id:long}")]
    [RequireDepartmentPermission(Permission.Works_View)]
    [ProducesResponseType(typeof(StudentWorkDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        var query = new GetStudentWorkByIdQuery { WorkId = id };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<StudentWorkDetailResponse>());
    }

    /// <summary>
    /// Get all student works for a department in an academic year.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of works</returns>
    [HttpGet("by-department")]
    [RequireDepartmentPermission(Permission.Works_View)]
    [ProducesResponseType(typeof(IReadOnlyList<StudentWorkResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDepartment(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentWorksByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<StudentWorkResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Get the current student's own works ("My Works").
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of student's works</returns>
    [HttpGet("my")]
    [RequirePermission(Permission.Works_ViewOwn)]
    [ProducesResponseType(typeof(IReadOnlyList<StudentWorkResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyWorks(CancellationToken cancellationToken = default)
    {
        var query = new GetMyWorkQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var response = result.Value.Adapt<IReadOnlyList<StudentWorkResponse>>();
        return Ok(response);
    }

    /// <summary>
    /// Create a new student work (typically after application is accepted).
    /// </summary>
    /// <param name="request">Work creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created work ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Works_Edit)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateStudentWorkRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<CreateStudentWorkCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Add a participant to a student work (team works).
    /// </summary>
    /// <param name="workId">Student work ID</param>
    /// <param name="request">Participant details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created participant ID</returns>
    [HttpPost("{workId:long}/participants")]
    [RequireDepartmentPermission(Permission.Works_ManageParticipants)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddParticipant(
        long workId,
        [FromBody] AddParticipantRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<AddParticipantCommand>() with { WorkId = workId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = workId }, result.Value);
    }

    /// <summary>
    /// Remove a participant from a student work.
    /// </summary>
    /// <param name="workId">Student work ID</param>
    /// <param name="studentId">Student ID to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{workId:long}/participants/{studentId:int}")]
    [RequireDepartmentPermission(Permission.Works_ManageParticipants)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveParticipant(
        long workId,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveParticipantCommand
        {
            WorkId = workId,
            StudentId = studentId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

}
