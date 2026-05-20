namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateProtocol;
using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Application.Features.Defense.Evaluation.Queries.GetProtocol;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Requests.Defense;
using AWM.Service.WebAPI.Common.Contracts.Responses.Defense;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for defense protocol management — generation and retrieval.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/protocols")]
[Produces("application/json")]
public sealed class ProtocolsController : BaseController
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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Protocol details</returns>
    [HttpGet("{protocolId:long}")]
    [RequireAccess("FinalDefense", "Read")]
    [ProducesResponseType(typeof(ProtocolResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(long protocolId, CancellationToken cancellationToken = default)
    {
        var query = new GetProtocolQuery { ProtocolId = protocolId };
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var dto = result.Value;
        var response = dto.Adapt<ProtocolResponse>();

        return Ok(response);
    }

    /// <summary>
    /// Generate a defense session protocol (Secretary action).
    /// </summary>
    /// <param name="request">Protocol generation details</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created protocol ID</returns>
    [HttpPost]
    [RequireAccess("Defense_Protocol", "Create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Generate([FromBody] GenerateProtocolRequest request, CancellationToken cancellationToken = default)
    {
        var command = request.Adapt<GenerateProtocolCommand>();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return CreatedAtAction(nameof(GetById), new { protocolId = result.Value }, result.Value);
    }
}
