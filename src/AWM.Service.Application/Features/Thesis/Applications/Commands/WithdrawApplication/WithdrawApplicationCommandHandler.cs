namespace AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for WithdrawApplicationCommand.
/// Allows student to withdraw (soft delete) their application.
/// </summary>
public sealed class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;

    public WithdrawApplicationCommandHandler(ITopicApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<Result> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
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
        if (application.StudentId != request.StudentId)
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
        application.Delete(request.StudentId);

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