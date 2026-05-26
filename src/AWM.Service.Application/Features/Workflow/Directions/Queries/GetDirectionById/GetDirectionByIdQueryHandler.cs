using AWM.Service.Application.Features.Workflow.Directions.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetDirectionById;

/// <summary>
/// Query handler for getting a single thesis direction by ID.
/// </summary>
public sealed class GetDirectionByIdQueryHandler : IRequestHandler<GetDirectionByIdQuery, Result<DirectionDto>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IWorkflowRepository _workflowRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDirectionByIdQueryHandler"/> class.
    /// </summary>
    public GetDirectionByIdQueryHandler(
        IDirectionRepository directionRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _employeeRepository = employeeRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _workflowRepository = workflowRepository;
    }

    /// <summary>
    /// Handles the request to get a direction by ID.
    /// </summary>
    public async Task<Result<DirectionDto>> Handle(GetDirectionByIdQuery request, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (direction == null)
        {
            return Result.Failure<DirectionDto>(new Error("Directions.NotFound", "Direction not found."));
        }

        var creatorId = direction.CreatedBy;
        var employee = await _employeeRepository.GetByUserIdAsync(creatorId, cancellationToken);
        var user = await _userReadOnlyRepository.GetByIdAsync(creatorId, cancellationToken);

        string fullName = "Unknown";
        string positionTitle = "";

        if (user != null)
        {
            fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
            var mainPosition = employee?.Positions?.FirstOrDefault(p => p.IsMainPosition) 
                               ?? employee?.Positions?.FirstOrDefault();
            positionTitle = mainPosition?.Position?.Title ?? "";
        }

        var state = await _workflowRepository.GetStateByIdAsync(direction.CurrentStateId, cancellationToken);
        var currentStateName = state?.SystemName ?? "";
        var currentStateDisplayName = state?.DisplayName ?? "";

        var dto = new DirectionDto(
            direction.Id,
            direction.OrgUnitId,
            direction.SemesterId,
            direction.WorkTypeId,
            direction.TitleRu,
            direction.TitleKz,
            direction.TitleEn,
            direction.DescriptionRu,
            direction.DescriptionKz,
            direction.DescriptionEn,
            direction.CurrentStateId,
            currentStateName,
            currentStateDisplayName,
            direction.SubmittedAt,
            direction.ReviewedAt,
            direction.ReviewedBy,
            direction.ReviewComment,
            direction.CreatedAt,
            direction.CreatedBy,
            fullName,
            positionTitle);

        return Result.Success(dto);
    }
}
