namespace AWM.Service.Application.Features.Admin.Users.Commands.ToggleUserStatus;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for ToggleUserStatusCommand.
/// Activates or deactivates the user based on the request.
/// </summary>
public sealed class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
