namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Workflow.WorkTypes.Commands.CreateWorkType;
using AWM.Service.Application.Features.Workflow.WorkTypes.Commands.UpdateWorkType;
using AWM.Service.Application.Features.Workflow.WorkTypes.Commands.DeleteWorkType;
using AWM.Service.Application.Features.Workflow.WorkTypes.Queries.GetWorkTypes;
using AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;
using AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class WorkTypesController : BaseController
{
    private readonly ISender _sender;
    public WorkTypesController(ISender sender) { _sender = sender; }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _sender.Send(new GetWorkTypesQuery(), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value.Adapt<IReadOnlyList<WorkTypeResponse>>());
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateWorkTypeRequest request, CancellationToken ct)
    {
        var cmd = request.Adapt<CreateWorkTypeCommand>();
        var result = await _sender.Send(cmd, ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkTypeRequest request, CancellationToken ct)
    {
        var cmd = request.Adapt<UpdateWorkTypeCommand>() with { Id = id };
        var result = await _sender.Send(cmd, ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteWorkTypeCommand(id), ct);
        if (result.IsFailed) return HandleResultError(result.Error);
        return Ok();
    }
}

