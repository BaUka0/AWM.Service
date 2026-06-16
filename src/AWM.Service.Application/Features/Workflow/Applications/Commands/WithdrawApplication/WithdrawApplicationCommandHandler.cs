using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.WithdrawApplication;

public sealed class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var studentId = _currentUserProvider.UserId.Value;

        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        if (application.StudentId != studentId)
            return Result.Failure(new Error("Applications.Unauthorized", "You can only withdraw your own applications."));

        if (!application.IsPending)
            return Result.Failure(new Error("Applications.InvalidState", "Only pending applications can be withdrawn."));

        await _applicationRepository.DeleteAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
