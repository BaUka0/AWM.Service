namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentDefenseStep;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStudentDefenseStepQueryHandler
    : IRequestHandler<GetStudentDefenseStepQuery, Result<StudentDefenseStepDto?>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPreDefenseAttemptRepository _preDefenseAttemptRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetStudentDefenseStepQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IStudentRepository studentRepository,
        IStudentWorkRepository workRepository,
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IPreDefenseAttemptRepository preDefenseAttemptRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _preDefenseAttemptRepository = preDefenseAttemptRepository ?? throw new ArgumentNullException(nameof(preDefenseAttemptRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<StudentDefenseStepDto?>> Handle(
        GetStudentDefenseStepQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<StudentDefenseStepDto?>(
                new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;
        long workId;

        if (request.WorkId.HasValue)
        {
            workId = request.WorkId.Value;
        }
        else
        {
            var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
            if (student is null)
            {
                return Result.Success<StudentDefenseStepDto?>(null);
            }

            var works = await _workRepository.GetByStudentAsync(student.Id, cancellationToken);
            var work = works.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
            if (work is null)
            {
                return Result.Success<StudentDefenseStepDto?>(null);
            }

            workId = work.Id;
        }

        var detailedWork = await _workRepository.GetByIdWithDetailsAsync(workId, cancellationToken);
        if (detailedWork is null)
        {
            return Result.Success<StudentDefenseStepDto?>(null);
        }

        var state = await _workflowRepository.GetStateByIdAsync(detailedWork.CurrentStateId, cancellationToken);
        var stateName = state?.SystemName?.ToLowerInvariant() ?? string.Empty;

        var stepType = stateName.Contains("defense") && !stateName.Contains("pre")
            ? "defense"
            : "pre-defense";

        var schedule = await _scheduleRepository.GetByWorkIdAsync(workId, cancellationToken);

        DefenseStepScheduleDto? scheduleDto = null;
        IReadOnlyList<DefenseStepMemberDto> commissionMembers = [];
        DefenseStepResultsDto? results = null;

        if (schedule is not null)
        {
            scheduleDto = new DefenseStepScheduleDto
            {
                Date = schedule.DefenseDate,
                Time = schedule.DefenseDate.ToString("HH:mm"),
                Location = schedule.Location
            };

            var commission = await _commissionRepository.GetByIdWithMembersAsync(schedule.CommissionId, cancellationToken);
            if (commission is not null)
            {
                var commissionUsers = await _userRepository.GetByIdsAsync(
                    commission.Members.Select(m => m.UserId).Distinct(),
                    cancellationToken);
                var commissionUsersById = commissionUsers.ToDictionary(u => u.Id);
                var members = new List<DefenseStepMemberDto>();
                foreach (var member in commission.Members)
                {
                    var memberUser = commissionUsersById.GetValueOrDefault(member.UserId);

                    members.Add(new DefenseStepMemberDto
                    {
                        Name = memberUser?.Login ?? memberUser?.Email ?? $"User {member.UserId}",
                        Role = member.RoleInCommission.ToString()
                    });
                }
                commissionMembers = members;
            }

            if (stepType == "defense")
            {
                var averageScore = schedule.GetAverageScore();

                results = new DefenseStepResultsDto
                {
                    FinalGrade = detailedWork.FinalGrade,
                    CommissionGrade = averageScore,
                    Comments = null
                };
            }
        }

        IReadOnlyList<DefenseStepAttemptDto> previousAttempts = [];
        int? attemptNumber = null;

        if (stepType == "pre-defense")
        {
            var attempts = await _preDefenseAttemptRepository.GetByWorkIdAsync(workId, cancellationToken);
            var orderedAttempts = attempts.OrderBy(a => a.PreDefenseNumber).ToList();

            previousAttempts = orderedAttempts
                .Select(a => new DefenseStepAttemptDto
                {
                    AttemptNumber = a.PreDefenseNumber,
                    Date = a.AttemptDate,
                    Score = a.AverageScore,
                    IsPassed = a.IsPassed,
                    Comments = null
                })
                .ToList();

            attemptNumber = orderedAttempts.LastOrDefault()?.PreDefenseNumber;
        }

        var dto = new StudentDefenseStepDto
        {
            StepType = stepType,
            AttemptNumber = attemptNumber,
            Schedule = scheduleDto,
            Commission = commissionMembers,
            PreviousAttempts = previousAttempts,
            Results = results
        };

        return Result.Success<StudentDefenseStepDto?>(dto);
    }
}
