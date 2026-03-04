namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.AssignExperts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class AssignExpertsCommandHandler : IRequestHandler<AssignExpertsCommand, Result<int>>
{
    private readonly IExpertRepository _expertRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignExpertsCommandHandler> _logger;

    public AssignExpertsCommandHandler(
        IExpertRepository expertRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<AssignExpertsCommandHandler> logger)
    {
        _expertRepository = expertRepository ?? throw new ArgumentNullException(nameof(expertRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(AssignExpertsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            if (!request.Assignments.Any())
                return Result.Failure<int>(new Error("400", "At least one expert assignment is required."));

            var existingExperts = await _expertRepository.GetByDepartmentAsync(
                request.DepartmentId, cancellationToken);

            var created = 0;
            foreach (var assignment in request.Assignments)
            {
                // Check for existing active expert with same user + type
                var existing = existingExperts.FirstOrDefault(
                    e => e.UserId == assignment.UserId
                         && e.ExpertiseType == assignment.ExpertiseType
                         && !e.IsDeleted);

                if (existing != null)
                {
                    if (!existing.IsActive)
                    {
                        existing.Activate();
                        await _expertRepository.UpdateAsync(existing, cancellationToken);
                    }
                    continue;
                }

                var expert = new Expert(
                    assignment.UserId,
                    request.DepartmentId,
                    assignment.ExpertiseType,
                    userId.Value);

                await _expertRepository.AddAsync(expert, cancellationToken);
                created++;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Assigned {Created} new experts for Dept={DeptId}", created, request.DepartmentId);
            return Result.Success(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignExperts failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
