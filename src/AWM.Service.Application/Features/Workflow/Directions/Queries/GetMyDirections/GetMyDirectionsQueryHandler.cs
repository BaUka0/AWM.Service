using AWM.Service.Application.Features.Workflow.Directions.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetMyDirections;

/// <summary>
/// Query handler for getting directions created by the current supervisor.
/// </summary>
public sealed class GetMyDirectionsQueryHandler : IRequestHandler<GetMyDirectionsQuery, Result<IReadOnlyList<DirectionSummaryDto>>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly ISemesterReadOnlyRepository _semesterReadOnlyRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMyDirectionsQueryHandler"/> class.
    /// </summary>
    public GetMyDirectionsQueryHandler(
        IDirectionRepository directionRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        ISemesterReadOnlyRepository semesterReadOnlyRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _directionRepository = directionRepository;
        _employeeRepository = employeeRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _semesterReadOnlyRepository = semesterReadOnlyRepository;
        _currentUserProvider = currentUserProvider;
    }

    /// <summary>
    /// Handles the request to get the supervisor's own directions.
    /// </summary>
    public async Task<Result<IReadOnlyList<DirectionSummaryDto>>> Handle(GetMyDirectionsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<DirectionSummaryDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // Resolve semesterId fallback if null
        int semesterId;
        if (request.SemesterId.HasValue)
        {
            semesterId = request.SemesterId.Value;
        }
        else
        {
            var currentSemester = await _semesterReadOnlyRepository.GetCurrentAsync(cancellationToken);
            if (currentSemester == null)
            {
                return Result.Failure<IReadOnlyList<DirectionSummaryDto>>(new Error("Directions.SemesterNotFound", "Active semester not found in system."));
            }
            semesterId = currentSemester.Id;
        }

        var directions = await _directionRepository.GetBySupervisorAsync(currentUserId, semesterId, cancellationToken);
        
        var creatorIds = directions.Select(d => d.CreatedBy).Distinct().ToList();
        var creators = new Dictionary<int, (string FullName, string PositionTitle)>();
        
        if (creatorIds.Any())
        {
            var employees = await _employeeRepository.GetByIdsAsync(creatorIds, cancellationToken);
            var users = await _userReadOnlyRepository.GetByIdsAsync(creatorIds, cancellationToken);
            
            foreach (var creatorId in creatorIds)
            {
                var user = users.FirstOrDefault(u => u.Id == creatorId);
                var employee = employees.FirstOrDefault(e => e.Id == creatorId);
                
                if (user != null)
                {
                    var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
                    var mainPosition = employee?.Positions?.FirstOrDefault(p => p.IsMainPosition) 
                                       ?? employee?.Positions?.FirstOrDefault();
                    creators[creatorId] = (fullName, mainPosition?.Position?.Title ?? "");
                }
                else
                {
                    creators[creatorId] = ("Unknown", "");
                }
            }
        }

        var resultList = directions.Select(d => new DirectionSummaryDto(
            d.Id,
            d.OrgUnitId,
            d.SemesterId,
            d.TitleRu,
            d.TitleKz,
            d.TitleEn,
            d.CurrentStateId,
            d.CreatedAt,
            d.CreatedBy,
            creators.TryGetValue(d.CreatedBy, out var info) ? info.FullName : "Unknown",
            creators.TryGetValue(d.CreatedBy, out var info2) ? info2.PositionTitle : ""
        )).ToList();

        return Result.Success<IReadOnlyList<DirectionSummaryDto>>(resultList);
    }
}
