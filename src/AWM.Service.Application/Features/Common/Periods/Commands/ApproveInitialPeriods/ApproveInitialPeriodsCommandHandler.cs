namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.CommonDomain.Services;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ApproveInitialStagesCommandHandler : IRequestHandler<ApproveInitialStagesCommand, Result>
{
    private readonly IStageRepository _stageRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveInitialStagesCommandHandler> _logger;

    public ApproveInitialStagesCommandHandler(
        IStageRepository stageRepository,
        IStaffRepository staffRepository,
        IAcademicProgramRepository academicProgramRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveInitialStagesCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _academicProgramRepository = academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(ApproveInitialStagesCommand request, CancellationToken cancellationToken)
    {
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
