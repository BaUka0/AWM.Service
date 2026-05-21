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

using System.Text.Json;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;

public sealed class AssignExpertsCommandHandler : IRequestHandler<AssignExpertsCommand, Result<int>>
{
    private readonly IStaffAssignmentRepository _assignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignExpertsCommandHandler> _logger;

    public AssignExpertsCommandHandler(
        IStaffAssignmentRepository assignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<AssignExpertsCommandHandler> logger)
    {
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
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

            // Get existing active expert assignments for this department
            var existingAssignments = await _assignmentRepository.GetByTargetAsync(
                "Department", request.DepartmentId, cancellationToken);

            var created = 0;
            foreach (var req in request.Assignments)
            {
                var metadata = JsonSerializer.Serialize(new { CheckTypeId = req.CheckTypeId });

                // Check for existing active assignment with same user + role + metadata CheckTypeId
                var existing = existingAssignments.FirstOrDefault(
                    a => a.UserId == req.UserId
                         && a.RoleType == StaffRoleType.QualityExpert
                         && ParseCheckTypeIdFromMetadata(a.MetadataJson) == req.CheckTypeId
                         && !a.IsDeleted);

                if (existing != null)
                {
                    if (!existing.IsActive)
                    {
                        existing.Activate(userId.Value);
                        await _assignmentRepository.UpdateAsync(existing, cancellationToken);
                    }
                    continue;
                }

                var assignment = new StaffAssignment(
                    req.UserId,
                    StaffRoleType.QualityExpert,
                    "Department",
                    request.DepartmentId,
                    userId.Value,
                    metadata);

                await _assignmentRepository.AddAsync(assignment, cancellationToken);
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

    private static int? ParseCheckTypeIdFromMetadata(string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson)) return null;
        try
        {
            using var doc = JsonDocument.Parse(metadataJson);
            if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop) && prop.ValueKind == JsonValueKind.Number)
            {
                return prop.GetInt32();
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
    }
}
