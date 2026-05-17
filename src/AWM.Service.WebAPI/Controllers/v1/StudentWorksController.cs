namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Common.Notifications.Commands.SendReadinessReminders;
using AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;
using AWM.Service.Application.Features.Thesis.Works.Commands.AssignReviewerToWork;
using AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;
using AWM.Service.Application.Features.Thesis.Works.Commands.SetRepositoryUrl;
using AWM.Service.Application.Features.Thesis.Works.Commands.RemoveParticipant;
using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetAssignedReviewer;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetAdmittedStudents;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetDefenseReadiness;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWork;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWorkProgress;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetReviewStatusByDepartment;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorkById;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksByDepartment;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetMySupervisedWorks;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentDefenseStep;
using AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksBySupervisor;
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
    [RequireAccess("StudentWorks", "Read")]
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
    /// Get the current supervisor's supervised works ("My Students").
    /// </summary>
    [HttpGet("my-supervised")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<SupervisedWorkResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMySupervisedWorks(
        [FromQuery] int? academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMySupervisedWorksQuery { AcademicYearId = academicYearId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value.Adapt<IReadOnlyList<SupervisedWorkResponse>>());
    }

    /// <summary>
    /// Get a student work by ID with full details.
    /// </summary>
    /// <param name="id">Student work ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed work information including participants</returns>
    [HttpGet("{id:long}")]
    [RequireAccess("StudentWorks", "Read")]
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
    [RequireAccess("StudentWorks", "Read")]
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
    [RequireAccess("StudentWorks", "Read")]
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
    /// Get the current student's defense step data (pre-defense or final defense schedule, commission, attempts, results).
    /// </summary>
    [HttpGet("my-defense-step")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(StudentDefenseStepResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyDefenseStep(
        [FromQuery] long? workId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentDefenseStepQuery { WorkId = workId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        if (result.Value is null)
            return Ok(null);

        return Ok(result.Value.Adapt<StudentDefenseStepResponse>());
    }

    /// <summary>
    /// Get the current student's work progress with full details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Student work progress or null if no work exists</returns>
    [HttpGet("my-progress")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(StudentWorkProgressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyWorkProgress(CancellationToken cancellationToken = default)
    {
        var query = new GetMyWorkProgressQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        if (result.Value is null)
            return Ok(null);

        var response = result.Value.Adapt<StudentWorkProgressResponse>();
        return Ok(response);
    }

    /// <summary>
    /// Create a new student work (typically after application is accepted).
    /// </summary>
    /// <param name="request">Work creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created work ID</returns>
    [HttpPost]
    [RequireAccess("StudentWorks", "Create")]
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
    [RequireAccess("StudentWorks", "Create")]
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
    [RequireAccess("StudentWorks", "Delete")]
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

    /// <summary>
    /// Assign an external reviewer to a student work.
    /// </summary>
    /// <param name="workId">Student work ID</param>
    /// <param name="request">Reviewer assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created review ID</returns>
    [HttpPost("{workId:long}/assign-reviewer")]
    [RequireAccess("Reviews", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignReviewer(
        long workId,
        [FromBody] AssignReviewerToWorkRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new AssignReviewerToWorkCommand
        {
            WorkId = workId,
            ReviewerId = request.ReviewerId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = workId }, result.Value);
    }

    /// <summary>
    /// Set the repository URL for a student work (for software check).
    /// </summary>
    /// <param name="workId">Student work ID</param>
    /// <param name="request">Repository URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPut("{workId:long}/repository-url")]
    [RequireAccess("StudentWorks", "Update")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetRepositoryUrl(
        long workId,
        [FromBody] SetRepositoryUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SetRepositoryUrlCommand
        {
            WorkId = workId,
            RepositoryUrl = request.RepositoryUrl
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Get the assigned reviewer for a student work.
    /// </summary>
    /// <param name="workId">Student work ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assigned reviewer details or null</returns>
    [HttpGet("{workId:long}/assigned-reviewer")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(AssignedReviewerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAssignedReviewer(
        long workId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAssignedReviewerQuery { WorkId = workId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get review status for all works in a department.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Review status summary</returns>
    [HttpGet("review-status")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(ReviewStatusByDepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReviewStatus(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetReviewStatusByDepartmentQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get defense readiness dashboard for a department.
    /// </summary>
    [HttpGet("defense-readiness")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(DefenseReadinessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDefenseReadiness(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDefenseReadinessQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get list of students admitted to final defense.
    /// </summary>
    [HttpGet("admitted-students")]
    [RequireAccess("StudentWorks", "Read")]
    [ProducesResponseType(typeof(IReadOnlyList<AdmittedStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAdmittedStudents(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdmittedStudentsQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Send readiness reminders to students with incomplete steps.
    /// </summary>
    [HttpPost("send-readiness-reminders")]
    [RequireAccess("StudentWorks", "Create")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendReadinessReminders(
        [FromBody] SendReadinessRemindersRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SendReadinessRemindersCommand
        {
            DepartmentId = request.DepartmentId,
            AcademicYearId = request.AcademicYearId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

}
