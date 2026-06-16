using System;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Works.DTOs;

public record SupervisedStudentDto(
    int StudentId,
    MultilingualTextDto Name,
    decimal? Score);

public record SupervisedFileDto(
    long Id,
    MultilingualTextDto Name,
    string Date,
    string UploadedBy);

public record SupervisedNoteDto(
    long Id,
    MultilingualTextDto Text,
    string Date);

public record SupervisedTopicDto(
    long Id,
    MultilingualTextDto Title);

public record QualityCheckSummaryDto(
    int CheckTypeId,
    string CheckTypeName,
    bool IsPassed,
    decimal? ResultValue,
    int AttemptNumber);

public record SupervisedWorkDto(
    long WorkId,
    string StageKey,
    string Stage,
    MultilingualTextDto TopicTitle,
    MultilingualTextDto DirectionTitle,
    IReadOnlyList<SupervisedStudentDto> Students,
    IReadOnlyList<SupervisedFileDto> ProjectFiles,
    IReadOnlyList<SupervisedFileDto> SupervisorFiles,
    IReadOnlyList<SupervisedNoteDto> Notes,
    SupervisedTopicDto Topic,
    bool IsAwaitingDepartmentApproval = false,
    IReadOnlyList<QualityCheckSummaryDto>? QualityChecksSummary = null);
