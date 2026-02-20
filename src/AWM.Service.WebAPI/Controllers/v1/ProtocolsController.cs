namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateProtocol;
using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Application.Features.Defense.Evaluation.Queries.GetProtocol;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for defense protocol management — generation and retrieval.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/protocols")]
[Produces("application/json")]
public class ProtocolsController : BaseController
{
    private readonly ISender _sender;

    public ProtocolsController(ISender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    /// <summary>
    /// Get a protocol by its ID.
    /// </summary>
    /// <param name="protocolId">Protocol ID</param>
    /// <returns>Protocol details</returns>
    [HttpGet("{protocolId:long}")]
    [RequireDepartmentPermission(Permission.Defense_View)]
    [ProducesResponseType(typeof(ProtocolDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long protocolId)
    {
        var query = new GetProtocolQuery { ProtocolId = protocolId };
        var result = await _sender.Send(query);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Generate a defense session protocol (Secretary action).
    /// </summary>
    /// <param name="request">Protocol generation details</param>
    /// <returns>Created protocol ID</returns>
    [HttpPost]
    [RequireDepartmentPermission(Permission.Defense_GenerateProtocol)]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] GenerateProtocolRequest request)
    {
        var command = new GenerateProtocolCommand
        {
            ScheduleId = request.ScheduleId,
            CommissionId = request.CommissionId
        };

        var result = await _sender.Send(command);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { protocolId = result.Value }, result.Value);
    }
}
