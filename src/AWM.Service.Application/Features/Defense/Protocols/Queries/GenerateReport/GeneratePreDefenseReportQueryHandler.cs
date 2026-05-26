using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;

public sealed class GeneratePreDefenseReportQueryHandler : IRequestHandler<GeneratePreDefenseReportQuery, Result<byte[]>>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly ISpecialityRepository _specialityRepository;
    private readonly IEvaluationCriteriaRepository _evaluationCriteriaRepository;
    private readonly IPdfReportService _pdfReportService;

    public GeneratePreDefenseReportQueryHandler(
        IProtocolRepository protocolRepository,
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository studentWorkRepository,
        IUserRepository userRepository,
        ICommissionRepository commissionRepository,
        ISpecialityRepository specialityRepository,
        IEvaluationCriteriaRepository evaluationCriteriaRepository,
        IPdfReportService _pdfReportService)
    {
        this._protocolRepository = protocolRepository;
        this._scheduleRepository = scheduleRepository;
        this._studentWorkRepository = studentWorkRepository;
        this._userRepository = userRepository;
        this._commissionRepository = commissionRepository;
        this._specialityRepository = specialityRepository;
        this._evaluationCriteriaRepository = evaluationCriteriaRepository;
        this._pdfReportService = _pdfReportService;
    }

    public async Task<Result<byte[]>> Handle(GeneratePreDefenseReportQuery request, CancellationToken cancellationToken)
    {
        // 1. Retrieve protocol
        var protocol = await _protocolRepository.GetByIdAsync(request.ProtocolId, cancellationToken);
        if (protocol == null)
        {
            return Result.Failure<byte[]>(new Error("Protocol.NotFound", $"Protocol with ID {request.ProtocolId} not found."));
        }

        // 2. Retrieve schedule
        var schedule = await _scheduleRepository.GetByIdAsync(protocol.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure<byte[]>(new Error("Schedule.NotFound", "Associated schedule not found."));
        }

        // 3. Retrieve student work
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(schedule.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<byte[]>(new Error("StudentWork.NotFound", "Student work not found."));
        }

        // 4. Retrieve student user details
        var studentParticipant = work.Participants.FirstOrDefault();
        string studentName = "Студент";
        if (studentParticipant != null)
        {
            var studentUser = await _userRepository.GetByIdAsync(studentParticipant.StudentId, cancellationToken);
            if (studentUser != null)
            {
                studentName = $"{studentUser.LastName} {studentUser.FirstName} {studentUser.MiddleName}".Trim();
            }
        }

        // 5. Retrieve speciality
        string specialityName = "Не указана";
        if (work.SpecialityId.HasValue)
        {
            var speciality = await _specialityRepository.GetByIdAsync(work.SpecialityId.Value, cancellationToken);
            if (speciality != null)
            {
                specialityName = $"{speciality.Code} - {speciality.Title}";
            }
        }

        // 6. Retrieve commission with assignments
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
        if (commission == null)
        {
            return Result.Failure<byte[]>(new Error("Commission.NotFound", "Commission not found."));
        }

        // 7. Retrieve commission members users
        var userIds = commission.Assignments.Select(a => a.UserId).Distinct().ToList();
        var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id);

        // 8. Retrieve evaluation criteria
        var criteriaList = await _evaluationCriteriaRepository.GetAllAsync(cancellationToken);
        var criteriaMap = criteriaList.ToDictionary(c => c.Id);

        // 9. Map grades
        var gradesData = new List<MemberGradeData>();
        foreach (var grade in schedule.Grades)
        {
            var assignment = commission.Assignments.FirstOrDefault(a => a.Id == grade.AssignmentId);
            string memberName = "Unknown Member";
            string memberRole = "Member";
            if (assignment != null)
            {
                memberRole = assignment.RoleType.ToString();
                if (userMap.TryGetValue(assignment.UserId, out var user))
                {
                    memberName = $"{user.LastName} {user.FirstName}".Trim();
                }
            }

            string criteriaName = "Критерий";
            if (criteriaMap.TryGetValue(grade.CriteriaId, out var criteria))
            {
                criteriaName = criteria.CriteriaName;
            }

            gradesData.Add(new MemberGradeData(
                memberName,
                memberRole,
                criteriaName,
                grade.Score,
                grade.Comment
            ));
        }

        // 10. Prepare report payload
        var reportData = new ProtocolReportData(
            protocol.Id,
            protocol.ProtocolNumber,
            commission.Name ?? "Комиссия предзащиты",
            "PreDefense",
            protocol.SessionDate.ToString("dd.MM.yyyy HH:mm"),
            studentName,
            work.TopicId.HasValue ? "Topic Work" : "Diplom Work", // Fallback, could query topic details if needed
            specialityName,
            protocol.FinalScoreNumeric ?? 0,
            protocol.FinalGradeLetter ?? "-",
            protocol.Decision ?? "-",
            protocol.Comments,
            gradesData
        );

        // 11. Render PDF
        var pdfBytes = await _pdfReportService.GenerateProtocolReportAsync(reportData);
        return Result.Success(pdfBytes);
    }
}
