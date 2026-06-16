namespace AWM.Service.Domain.CommonDomain.Constants;

/// <summary>
/// Centralized constants for Workflow Stage IDs.
/// Use these instead of hardcoded magic numbers in handlers and seeding.
/// </summary>
public static class WorkflowStageIds
{
    public const int DirectionProposal = 1;
    public const int TopicProposal = 2;
    public const int TopicPreparation = 3;
    public const int Preparation = 4;

    public const int PreDefense1 = 5;
    public const int PreDefense2 = 6;
    public const int PreDefense3 = 7;
    public const int FinalDefense = 8;

    public const int ChecksPeriod = 9;
}
