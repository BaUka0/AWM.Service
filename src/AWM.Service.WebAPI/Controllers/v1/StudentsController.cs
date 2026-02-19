namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Commands.Students.CreateStudent;
using AWM.Service.Application.Features.Edu.Commands.Students.UpdateStudent;
using AWM.Service.Application.Features.Edu.Queries.Students.GetStudentById;
using AWM.Service.Application.Features.Edu.Queries.Students.GetStudentsByProgram;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Students.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class StudentsController : BaseController
{
    private readonly ISender _sender;

    public StudentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a student by ID.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Student details</returns>
    [HttpGet("{studentId}")]
    [RequireDepartmentPermission(Permission.Students_View)]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int studentId, CancellationToken cancellationToken = default)
    {
        var query = new GetStudentByIdQuery
        {
            StudentId = studentId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var dto = result.Value;
        var response = new StudentResponse
        {
            Id = dto.Id,
            UserId = dto.UserId,
            FullName = dto.FullName,
            Email = dto.Email,
            GroupCode = dto.GroupCode,
            ProgramId = dto.ProgramId,
            ProgramName = dto.ProgramName,
            AdmissionYear = dto.AdmissionYear,
            CurrentCourse = dto.CurrentCourse,
            Status = dto.Status
        };

        return Ok(response);
    }

    /// <summary>
    /// Get students by academic program.
    /// </summary>
    /// <param name="programId">Program ID</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of students in the program</returns>
    [HttpGet]
    [RequireDepartmentPermission(Permission.Students_View)]
    [ProducesResponseType(typeof(IReadOnlyList<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByProgram([FromQuery] int programId, CancellationToken cancellationToken = default)
    {
        var query = new GetStudentsByProgramQuery
        {
            ProgramId = programId
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        var response = result.Value
            .Select(dto => new StudentResponse
            {
                Id = dto.Id,
                UserId = dto.UserId,
                FullName = dto.FullName,
                Email = dto.Email,
                GroupCode = dto.GroupCode,
                ProgramId = dto.ProgramId,
                ProgramName = dto.ProgramName,
                AdmissionYear = dto.AdmissionYear,
                CurrentCourse = dto.CurrentCourse,
                Status = dto.Status
            })
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Create a new student.
    /// </summary>
    /// <param name="request">Create student request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created student ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Students_Create)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateStudentCommand
        {
            UserId = request.UserId,
            ProgramId = request.ProgramId,
            AdmissionYear = request.AdmissionYear,
            CurrentCourse = request.CurrentCourse,
            GroupCode = request.GroupCode
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { studentId = result.Value, version = "1.0" }, result.Value);
    }

    /// <summary>
    /// Update an existing student.
    /// </summary>
    /// <param name="studentId">Student ID</param>
    /// <param name="request">Update student request</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success</returns>
    [HttpPut("{studentId}")]
    [RequireDepartmentPermission(Permission.Students_Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int studentId, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateStudentCommand
        {
            StudentId = studentId,
            ProgramId = request.ProgramId,
            GroupCode = request.GroupCode,
            CurrentCourse = request.CurrentCourse
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
