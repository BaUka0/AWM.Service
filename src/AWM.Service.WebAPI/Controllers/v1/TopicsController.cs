namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;
using AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsByDirection;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing Thesis Topics.
/// </summary>
[ApiController]
[Route("api/v1/topics")]
[Produces("application/json")]
public class TopicsController : ControllerBase
{
    private readonly ISender _sender;

    public TopicsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get a specific topic by ID with full details.
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>Topic details with applications</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Application.Features.Thesis.Topics.DTOs.TopicDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long id)
    {
        var query = new GetTopicByIdQuery { TopicId = id };
        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get topics available for student selection.
    /// </summary>
    /// <param name="departmentId">Department ID</param>
    /// <param name="academicYearId">Academic year ID</param>
    /// <returns>List of available topics</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Thesis.Topics.DTOs.TopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] int departmentId,
        [FromQuery] int academicYearId)
    {
        var query = new GetAvailableTopicsQuery
        {
            DepartmentId = departmentId,
            AcademicYearId = academicYearId
        };

        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get topics by research direction.
    /// </summary>
    /// <param name="directionId">Direction ID</param>
    /// <returns>List of topics linked to the direction</returns>
    [HttpGet("by-direction/{directionId}")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Thesis.Topics.DTOs.TopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByDirection(long directionId)
    {
        var query = new GetTopicsByDirectionQuery { DirectionId = directionId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new thesis topic.
    /// </summary>
    /// <param name="request">Create topic request</param>
    /// <returns>Created topic ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] Common.Contracts.Requests.Thesis.CreateTopicRequest request)
    {
        var command = new CreateTopicCommand
        {
            DepartmentId = request.DepartmentId,
            SupervisorId = request.SupervisorId,
            AcademicYearId = request.AcademicYearId,
            WorkTypeId = request.WorkTypeId,
            DirectionId = request.DirectionId,
            TitleRu = request.TitleRu,
            TitleKz = request.TitleKz,
            TitleEn = request.TitleEn,
            Description = request.Description,
            MaxParticipants = request.MaxParticipants,
            CreatedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update an existing thesis topic.
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <param name="request">Update topic request</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(long id, [FromBody] Common.Contracts.Requests.Thesis.UpdateTopicRequest request)
    {
        var command = new UpdateTopicCommand
        {
            TopicId = id,
            TitleRu = request.TitleRu,
            TitleKz = request.TitleKz,
            TitleEn = request.TitleEn,
            Description = request.Description,
            MaxParticipants = request.MaxParticipants,
            ModifiedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Approve a thesis topic (department action).
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Approve(long id)
    {
        var command = new ApproveTopicCommand
        {
            TopicId = id,
            ApprovedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Close a thesis topic (no more applications accepted).
    /// </summary>
    /// <param name="id">Topic ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Close(long id)
    {
        var command = new CloseTopicCommand
        {
            TopicId = id,
            ClosedBy = GetCurrentUserId() // TODO: Implement user context
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
        {
            return HandleError(result.Error);
        }

        return NoContent();
    }

    #region Helper Methods

    /// <summary>
    /// Handles errors from FluentResult and returns appropriate HTTP status codes.
    /// </summary>
    private IActionResult HandleError(KDS.Primitives.FluentResult.Error error)
    {
        return error.Code switch
        {
            var code when code.StartsWith("NotFound") => NotFound(new { error.Code, error.Message }),
            var code when code.StartsWith("Validation") => BadRequest(new { error.Code, error.Message }),
            var code when code.StartsWith("BusinessRule") => Conflict(new { error.Code, error.Message }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error.Code, error.Message })
        };
    }

    /// <summary>
    /// Gets current user ID from authentication context.
    /// TODO: Implement actual user context retrieval.
    /// </summary>
    private int GetCurrentUserId()
    {
        return 1; // Placeholder
    }

    #endregion
}