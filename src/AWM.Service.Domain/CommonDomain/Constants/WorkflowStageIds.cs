namespace AWM.Service.Domain.CommonDomain.Constants;

/// <summary>
/// Centralized constants for Workflow Stage IDs.
/// Use these instead of hardcoded magic numbers in handlers and seeding.
/// </summary>
public static class WorkflowStageIds
{
    // Initial Stages
    public const int TopicProposal = 1;
    public const int TopicPreparation = 2;
    public const int Preparation = 3;

    // Defense Stages
    public const int PreDefense1 = 4;
    public const int PreDefense2 = 5;
    public const int PreDefense3 = 6;
    public const int FinalDefense = 7;
}
