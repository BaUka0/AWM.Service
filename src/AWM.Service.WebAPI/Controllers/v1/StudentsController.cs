namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Edu.Students.Commands.CreateStudent;
using AWM.Service.Application.Features.Edu.Students.Commands.UpdateStudent;
using AWM.Service.Application.Features.Edu.Students.Queries.GetStudentById;
using AWM.Service.Application.Features.Edu.Students.Queries.GetStudentsByProgram;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Edu;
using AWM.Service.WebAPI.Common.Contracts.Responses.Edu;
using Mapster;
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

        var response = result.Value.Adapt<StudentResponse>();

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

        var response = result.Value.Adapt<IReadOnlyList<StudentResponse>>();

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
        var command = request.Adapt<CreateStudentCommand>();

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
        var command = request.Adapt<UpdateStudentCommand>() with { StudentId = studentId };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return NoContent();
    }
}
