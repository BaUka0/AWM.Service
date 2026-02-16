using AWM.Service.Application.Features.Edu.Commands.Students.CreateStudent;
using AWM.Service.Application.Features.Edu.Commands.Students.UpdateStudent;
using AWM.Service.Application.Features.Edu.Queries.Students.GetStudentById;
using AWM.Service.Application.Features.Edu.Queries.Students.GetStudentsByProgram;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing Student profiles.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class StudentsController : BaseController
{
    private readonly ISender _sender;

    public StudentsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get students by academic program.
    /// </summary>
    /// <param name="programId">Academic Program ID</param>
    /// <returns>List of students</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Edu.DTOs.StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int programId)
    {
        var query = new GetStudentsByProgramQuery { ProgramId = programId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific student by ID.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <returns>Student details</returns>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(Application.Features.Edu.DTOs.StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int studentId)
    {
        var query = new GetStudentByIdQuery { StudentId = studentId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new student profile.
    /// </summary>
    /// <param name="request">Create student request</param>
    /// <returns>Created student ID</returns>
    [HttpPost]
    [RequirePermission(Permission.Users_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        var command = new CreateStudentCommand
        {
            UserId = request.UserId,
            ProgramId = request.ProgramId,
            AdmissionYear = request.AdmissionYear,
            CurrentCourse = request.CurrentCourse,
            GroupCode = request.GroupCode
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { studentId = result.Value, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing student profile.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="request">Update student request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{studentId}")]
    [RequirePermission(Permission.Users_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int studentId, [FromBody] UpdateStudentRequest request)
    {
        var command = new UpdateStudentCommand
        {
            StudentId = studentId,
            ProgramId = request.ProgramId,
            GroupCode = request.GroupCode,
            CurrentCourse = request.CurrentCourse
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }
}
