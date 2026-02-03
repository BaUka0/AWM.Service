namespace AWM.Service.Domain.CommonDomain.Enums;

/// <summary>
/// Workflow stages for period management.
/// Determines when specific operations are allowed in the system.
/// </summary>
public enum WorkflowStage
{
    /// <summary>
    /// Period for submitting research directions.
    /// </summary>
    DirectionSubmission,

    /// <summary>
    /// Period for creating topics within approved directions.
    /// </summary>
    TopicCreation,

    /// <summary>
    /// Period for students to select topics.
    /// </summary>
    TopicSelection,

    /// <summary>
    /// First pre-defense period.
    /// </summary>
    PreDefense1,

    /// <summary>
    /// Second pre-defense period.
    /// </summary>
    PreDefense2,

    /// <summary>
    /// Third pre-defense period (retake).
    /// </summary>
    PreDefense3,

    /// <summary>
    /// Final defense period.
    /// </summary>
    FinalDefense
}
