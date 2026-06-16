namespace AWM.Service.Application.Features.Workflow.Works.DTOs;

public sealed record DefenseReadinessDto(
    long WorkId,
    string StudentName,
    string TopicTitle,
    bool PreDefensePassed,
    bool NormocontrolPassed,
    bool AntiplagiarismPassed,
    bool ReviewPassed,
    bool SupervisorReviewPassed,
    bool Admitted,
    string CurrentState,
    bool SoftwareCheckPassed
);
