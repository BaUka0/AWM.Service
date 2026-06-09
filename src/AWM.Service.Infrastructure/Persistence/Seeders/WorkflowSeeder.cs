namespace AWM.Service.Infrastructure.Persistence.Seeders;

using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Wf.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Seeds workflow reference data: stages, work types, states, and transitions.
/// </summary>
internal sealed class WorkflowSeeder
{
    private readonly ApplicationDbContext _context;

    public WorkflowSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await SeedWorkflowStagesAsync(ct);
        await SeedWorkTypesAndStatesAsync(ct);
    }

    private async Task SeedWorkflowStagesAsync(CancellationToken ct)
    {
        if (!await _context.WorkflowStages.AnyAsync(ct))
        {
            _context.WorkflowStages.AddRange(
                new WorkflowStage("DirectionProposal", 1),
                new WorkflowStage("TopicProposal", 2),
                new WorkflowStage("TopicPreparation", 3),
                new WorkflowStage("Preparation", 4),
                new WorkflowStage("PreDefense1", 5),
                new WorkflowStage("PreDefense2", 6),
                new WorkflowStage("PreDefense3", 7),
                new WorkflowStage("FinalDefense", 8)
            );
            await _context.SaveChangesAsync(ct);
        }

        if (!await _context.WorkflowStages.AnyAsync(ws => ws.Id == WorkflowStageIds.ChecksPeriod, ct))
        {
            _context.WorkflowStages.Add(
                new WorkflowStage("ChecksPeriod", WorkflowStageIds.ChecksPeriod));
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task SeedWorkTypesAndStatesAsync(CancellationToken ct)
    {
        if (await _context.WorkTypes.AnyAsync(ct)) return;

        var courseWork = new WorkType("CourseWork");
        var diplomaWork = new WorkType("DiplomaWork");
        var masterThesis = new WorkType("MasterThesis");
        var phd = new WorkType("PhD");

        _context.WorkTypes.AddRange(courseWork, diplomaWork, masterThesis, phd);
        await _context.SaveChangesAsync(ct);

        await SeedCourseWorkStatesAsync(courseWork.Id, ct);
        await SeedThesisStatesAsync(diplomaWork.Id, ct);
        await SeedThesisStatesAsync(masterThesis.Id, ct);
        await SeedThesisStatesAsync(phd.Id, ct);
    }

    private async Task SeedCourseWorkStatesAsync(int workTypeId, CancellationToken ct)
    {
        var states = new[]
        {
            new State(workTypeId, "Draft",       0, "Черновик"),
            new State(workTypeId, "Submitted",   0, "Отправлен на проверку"),
            new State(workTypeId, "UnderReview", 0, "На проверке"),
            new State(workTypeId, "Approved",    0, "Одобрен",   isFinal: true),
            new State(workTypeId, "Rejected",    0, "Отклонён",  isFinal: true),
            new State(workTypeId, "Cancelled",   0, "Отменён",   isFinal: true),
        };
        _context.States.AddRange(states);
        await _context.SaveChangesAsync(ct);

        var s = states.ToDictionary(x => x.SystemName);
        _context.Transitions.AddRange(
            Transition.Automatic(s["Draft"].Id, s["Submitted"].Id),
            Transition.Automatic(s["Submitted"].Id, s["UnderReview"].Id),
            Transition.Automatic(s["UnderReview"].Id, s["Approved"].Id),
            Transition.Automatic(s["UnderReview"].Id, s["Rejected"].Id)
        );
        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedThesisStatesAsync(int workTypeId, CancellationToken ct)
    {
        var directionStates = new[]
        {
            new State(workTypeId, DirectionStates.Draft,             0, "Черновик направления"),
            new State(workTypeId, DirectionStates.Submitted,         0, "Направление отправлено на согласование"),
            new State(workTypeId, DirectionStates.Approved,          0, "Направление утверждено",     isFinal: true),
            new State(workTypeId, DirectionStates.Rejected,          0, "Направление отклонено",      isFinal: true),
            new State(workTypeId, DirectionStates.RequiresRevision,  0, "Требует доработки"),
        };

        var workStates = new[]
        {
            new State(workTypeId, WorkStates.Draft,                         0, "Черновик"),

            new State(workTypeId, WorkStates.PreDefense1WaitingForFiles,    0, "ПЗ-1: Ожидание документов"),
            new State(workTypeId, WorkStates.PreDefense1WaitingForSchedule, 0, "ПЗ-1: Ожидание расписания"),
            new State(workTypeId, WorkStates.PreDefense1Scheduled,          0, "ПЗ-1: Запланирована"),
            new State(workTypeId, WorkStates.PreDefense1Passed,             0, "ПЗ-1: Пройдена"),
            new State(workTypeId, WorkStates.PreDefense1Failed,             0, "ПЗ-1: Не пройдена"),

            new State(workTypeId, WorkStates.PreDefense2WaitingForFiles,    0, "ПЗ-2: Ожидание документов"),
            new State(workTypeId, WorkStates.PreDefense2WaitingForSchedule, 0, "ПЗ-2: Ожидание расписания"),
            new State(workTypeId, WorkStates.PreDefense2Scheduled,          0, "ПЗ-2: Запланирована"),
            new State(workTypeId, WorkStates.PreDefense2Passed,             0, "ПЗ-2: Пройдена"),
            new State(workTypeId, WorkStates.PreDefense2Failed,             0, "ПЗ-2: Не пройдена"),

            new State(workTypeId, WorkStates.PreDefense3WaitingForFiles,    0, "ПЗ-3: Ожидание документов"),
            new State(workTypeId, WorkStates.PreDefense3WaitingForSchedule, 0, "ПЗ-3: Ожидание расписания"),
            new State(workTypeId, WorkStates.PreDefense3Scheduled,          0, "ПЗ-3: Запланирована"),
            new State(workTypeId, WorkStates.PreDefense3Passed,             0, "ПЗ-3: Пройдена"),
            new State(workTypeId, WorkStates.PreDefense3Failed,             0, "ПЗ-3: Не пройдена (недопуск)"),

            new State(workTypeId, WorkStates.ChecksWaitingForInitial,       0, "Нормоконтроль"),
            new State(workTypeId, WorkStates.ChecksWaitingForAntiPlagiarism,0, "Антиплагиат"),
            new State(workTypeId, WorkStates.ReviewsWaitingForSupervisor,   0, "Ожидание отзыва руководителя"),
            new State(workTypeId, WorkStates.ReviewsWaitingForReviewer,     0, "Ожидание рецензии"),

            new State(workTypeId, WorkStates.ReadyForDefense,               0, "Допущен к защите"),
            new State(workTypeId, WorkStates.DefenseWaitingForSchedule,     0, "Защита: ожидание расписания"),
            new State(workTypeId, WorkStates.DefenseScheduled,              0, "Защита запланирована"),
            new State(workTypeId, WorkStates.Defended,                      0, "Защищён",          isFinal: true),
            new State(workTypeId, WorkStates.DefenseFailed,                 0, "Защита не сдана",  isFinal: true),
            new State(workTypeId, WorkStates.Cancelled,                     0, "Отменён/Недопуск", isFinal: true),
        };

        _context.States.AddRange(directionStates);
        _context.States.AddRange(workStates);
        await _context.SaveChangesAsync(ct);

        var d = directionStates.ToDictionary(s => s.SystemName);
        var w = workStates.ToDictionary(s => s.SystemName);

        var transitions = new List<Transition>
        {
            Transition.Automatic(d[DirectionStates.Draft].Id,            d[DirectionStates.Submitted].Id),
            Transition.Automatic(d[DirectionStates.Submitted].Id,        d[DirectionStates.Approved].Id),
            Transition.Automatic(d[DirectionStates.Submitted].Id,        d[DirectionStates.Rejected].Id),
            Transition.Automatic(d[DirectionStates.Submitted].Id,        d[DirectionStates.RequiresRevision].Id),
            Transition.Automatic(d[DirectionStates.RequiresRevision].Id, d[DirectionStates.Submitted].Id),

            Transition.Automatic(w[WorkStates.Draft].Id, w[WorkStates.PreDefense1WaitingForFiles].Id),

            Transition.Automatic(w[WorkStates.PreDefense1WaitingForFiles].Id,    w[WorkStates.PreDefense1WaitingForSchedule].Id),
            Transition.Automatic(w[WorkStates.PreDefense1WaitingForSchedule].Id, w[WorkStates.PreDefense1Scheduled].Id),
            Transition.Automatic(w[WorkStates.PreDefense1Scheduled].Id,          w[WorkStates.PreDefense1Passed].Id),
            Transition.Automatic(w[WorkStates.PreDefense1Scheduled].Id,          w[WorkStates.PreDefense1Failed].Id),
            Transition.Automatic(w[WorkStates.PreDefense1Passed].Id, w[WorkStates.PreDefense2WaitingForFiles].Id),
            Transition.Automatic(w[WorkStates.PreDefense1Failed].Id, w[WorkStates.PreDefense2WaitingForFiles].Id),

            Transition.Automatic(w[WorkStates.PreDefense2WaitingForFiles].Id,    w[WorkStates.PreDefense2WaitingForSchedule].Id),
            Transition.Automatic(w[WorkStates.PreDefense2WaitingForSchedule].Id, w[WorkStates.PreDefense2Scheduled].Id),
            Transition.Automatic(w[WorkStates.PreDefense2Scheduled].Id,          w[WorkStates.PreDefense2Passed].Id),
            Transition.Automatic(w[WorkStates.PreDefense2Scheduled].Id,          w[WorkStates.PreDefense2Failed].Id),
            Transition.Automatic(w[WorkStates.PreDefense2Passed].Id, w[WorkStates.ChecksWaitingForInitial].Id),
            Transition.Automatic(w[WorkStates.PreDefense2Failed].Id, w[WorkStates.PreDefense3WaitingForFiles].Id),

            Transition.Automatic(w[WorkStates.PreDefense3WaitingForFiles].Id,    w[WorkStates.PreDefense3WaitingForSchedule].Id),
            Transition.Automatic(w[WorkStates.PreDefense3WaitingForSchedule].Id, w[WorkStates.PreDefense3Scheduled].Id),
            Transition.Automatic(w[WorkStates.PreDefense3Scheduled].Id,          w[WorkStates.PreDefense3Passed].Id),
            Transition.Automatic(w[WorkStates.PreDefense3Scheduled].Id,          w[WorkStates.PreDefense3Failed].Id),
            Transition.Automatic(w[WorkStates.PreDefense3Passed].Id, w[WorkStates.ChecksWaitingForInitial].Id),
            Transition.Automatic(w[WorkStates.PreDefense3Failed].Id, w[WorkStates.Cancelled].Id),

            Transition.Automatic(w[WorkStates.ChecksWaitingForInitial].Id,        w[WorkStates.ChecksWaitingForAntiPlagiarism].Id),
            Transition.Automatic(w[WorkStates.ChecksWaitingForAntiPlagiarism].Id, w[WorkStates.ReviewsWaitingForSupervisor].Id),
            Transition.Automatic(w[WorkStates.ReviewsWaitingForSupervisor].Id,    w[WorkStates.ReviewsWaitingForReviewer].Id),
            Transition.Automatic(w[WorkStates.ReviewsWaitingForReviewer].Id,      w[WorkStates.ReadyForDefense].Id),

            Transition.Automatic(w[WorkStates.ReadyForDefense].Id,           w[WorkStates.DefenseWaitingForSchedule].Id),
            Transition.Automatic(w[WorkStates.DefenseWaitingForSchedule].Id, w[WorkStates.DefenseScheduled].Id),
            Transition.Automatic(w[WorkStates.DefenseScheduled].Id,          w[WorkStates.Defended].Id),
            Transition.Automatic(w[WorkStates.DefenseScheduled].Id,          w[WorkStates.DefenseFailed].Id),
        };

        _context.Transitions.AddRange(transitions);
        await _context.SaveChangesAsync(ct);
    }
}
