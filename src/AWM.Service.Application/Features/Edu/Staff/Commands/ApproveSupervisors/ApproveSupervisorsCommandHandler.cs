namespace AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;

using AWM.Service.Domain.University;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

public sealed class ApproveSupervisorsCommandHandler : IRequestHandler<ApproveSupervisorsCommand, Result>
{
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveSupervisorsCommandHandler> _logger;

    public ApproveSupervisorsCommandHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<ApproveSupervisorsCommandHandler> logger)
    {
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userAccessRepository = userAccessRepository ?? throw new ArgumentNullException(nameof(userAccessRepository));
        _roleAccessRepository = roleAccessRepository ?? throw new ArgumentNullException(nameof(roleAccessRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(ApproveSupervisorsCommand request, CancellationToken cancellationToken)
    {
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
