namespace AWM.Service.Application.Features.Workflow.Queries.GetAvailableTransitions;

using AWM.Service.Application.Features.Workflow.DTOs;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Services;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetAvailableTransitionsQueryHandler : IRequestHandler<GetAvailableTransitionsQuery, Result<IReadOnlyList<TransitionDto>>>
{
    private readonly IStateMachine _stateMachine;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;

    public GetAvailableTransitionsQueryHandler(
        IStateMachine stateMachine,
        IDirectionRepository directionRepository,
        IStudentWorkRepository studentWorkRepository)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _studentWorkRepository = studentWorkRepository ?? throw new ArgumentNullException(nameof(studentWorkRepository));
    }

    public async Task<Result<IReadOnlyList<TransitionDto>>> Handle(GetAvailableTransitionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            int currentStateId = request.EntityType.ToLowerInvariant() switch
            {
                "direction" => await GetDirectionStateAsync(request.EntityId, cancellationToken),
                "studentwork" or "work" => await GetStudentWorkStateAsync(request.EntityId, cancellationToken),
                _ => -1
            };

            if (currentStateId <= 0)
                return Result.Failure<IReadOnlyList<TransitionDto>>(new Error(DomainErrors.Workflow.EntityNotFound, $"Entity '{request.EntityType}' with ID {request.EntityId} not found."));

            var transitions = request.RoleId.HasValue
                ? await _stateMachine.GetAvailableTransitionsForRoleAsync(currentStateId, request.RoleId.Value, cancellationToken)
                : await _stateMachine.GetAvailableTransitionsAsync(currentStateId, cancellationToken);

            var dtos = new List<TransitionDto>();
            foreach (var transition in transitions)
            {
                var fromState = await _stateMachine.GetStateAsync(transition.FromStateId, cancellationToken);
                var toState = await _stateMachine.GetStateAsync(transition.ToStateId, cancellationToken);

                dtos.Add(new TransitionDto
                {
                    Id = transition.Id,
                    FromStateId = transition.FromStateId,
                    FromStateName = fromState?.DisplayName ?? fromState?.SystemName,
                    ToStateId = transition.ToStateId,
                    ToStateName = toState?.DisplayName ?? toState?.SystemName,
                    AllowedRoleId = transition.AllowedRoleId,
                    IsAutomatic = transition.IsAutomatic
                });
            }

            return Result.Success<IReadOnlyList<TransitionDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TransitionDto>>(new Error(DomainErrors.General.InternalError, $"An error occurred: {ex.Message}"));
        }
    }

    private async Task<int> GetDirectionStateAsync(long entityId, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(entityId, cancellationToken);
        return direction is null || direction.IsDeleted ? -1 : direction.CurrentStateId;
    }

    private async Task<int> GetStudentWorkStateAsync(long entityId, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdAsync(entityId, cancellationToken);
        return work is null || work.IsDeleted ? -1 : work.CurrentStateId;
    }
}
