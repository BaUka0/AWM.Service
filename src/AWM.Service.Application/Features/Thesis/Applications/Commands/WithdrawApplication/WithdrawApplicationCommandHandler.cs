namespace AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for WithdrawApplicationCommand.
/// Allows student to withdraw (soft delete) their application.
/// </summary>
public sealed class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public WithdrawApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // 1. Get application
        var application = await _applicationRepository.GetByIdAsync(
            request.ApplicationId,
            cancellationToken);

        if (application is null)
        {
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is already deleted
        if (application.IsDeleted)
        {
            return Result.Failure(new Error("Application.AlreadyDeleted", "This application has already been withdrawn."));
        }

        // 3. Check authorization - only the student who created the application can withdraw it
        if (application.StudentId != userId)
        {
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
        application.Delete(userId);

        // 6. Update application
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Database.Error", $"Failed to withdraw application: {ex.Message}"));
        }
    }
}