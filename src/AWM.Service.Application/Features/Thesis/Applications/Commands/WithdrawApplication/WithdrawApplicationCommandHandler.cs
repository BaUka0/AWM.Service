namespace AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for WithdrawApplicationCommand.
/// Allows student to withdraw (soft delete) their application.
/// </summary>
public sealed class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WithdrawApplicationCommandHandler> _logger;

    public WithdrawApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<WithdrawApplicationCommandHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to withdraw application ID={ApplicationId} by User={UserId}", request.ApplicationId, userId);

        if (!userId.HasValue)
        {
            _logger.LogWarning("WithdrawApplication failed: User identity could not be determined.");
            return Result.Failure(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var studentUserId = userId.Value;

        // 1. Get application
        var application = await _applicationRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (application is null)
        {
            _logger.LogWarning("WithdrawApplication failed: Application ID={ApplicationId} not found.", request.ApplicationId);
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is already deleted
        if (application.IsDeleted)
        {
            _logger.LogWarning("WithdrawApplication failed: Application ID={ApplicationId} is already withdrawn.", request.ApplicationId);
            return Result.Failure(new Error("Application.AlreadyDeleted", "This application has already been withdrawn."));
        }

        // 3. Check authorization - only the student who created the application can withdraw it
        if (application.StudentId != studentUserId)
        {
            _logger.LogWarning("WithdrawApplication failed: User={UserId} is not the owner of Application={ApplicationId}", studentUserId, request.ApplicationId);
            return Result.Failure(new Error("Authorization.Forbidden", "You can only withdraw your own applications."));
        }

        // 4. Check if application is still pending (optional business rule)
        // Uncomment if students should only be able to withdraw pending applications
        /*
        if (!application.IsPending)
        {
            return Result.Failure(new Error("Application.CannotWithdraw", 
                "Only pending applications can be withdrawn. This application has already been reviewed."));
        }
        */

        // 5. Withdraw the application (soft delete)
        application.Delete(studentUserId);

        // 6. Update application
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully withdrawn application ID={ApplicationId} by User={UserId}", request.ApplicationId, studentUserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WithdrawApplication failed for Application ID={ApplicationId}", request.ApplicationId);
            return Result.Failure(new Error("Database.Error", $"Failed to withdraw application: {ex.Message}"));
        }
    }
}