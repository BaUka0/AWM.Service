using System;
using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Works;

public record SupervisedStudentResponse(
    int StudentId,
    MultilingualTextResponse Name,
    decimal? Score);

public record SupervisedFileResponse(
    long Id,
    MultilingualTextResponse Name,
    string Date,
    string UploadedBy);

public record SupervisedNoteResponse(
    long Id,
    MultilingualTextResponse Text,
    string Date);

public record SupervisedTopicResponse(
    long Id,
    MultilingualTextResponse Title);

public record QualityCheckSummaryResponse(
    int CheckTypeId,
    string CheckTypeName,
    bool IsPassed,
    decimal? ResultValue,
    int AttemptNumber);

public record SupervisedWorkResponse(
    long WorkId,
    string StageKey,
    string Stage,
    MultilingualTextResponse TopicTitle,
    MultilingualTextResponse DirectionTitle,
    IReadOnlyList<SupervisedStudentResponse> Students,
    IReadOnlyList<SupervisedFileResponse> ProjectFiles,
    IReadOnlyList<SupervisedFileResponse> SupervisorFiles,
    IReadOnlyList<SupervisedNoteResponse> Notes,
    SupervisedTopicResponse Topic,
    bool IsAwaitingDepartmentApproval = false,
    IReadOnlyList<QualityCheckSummaryResponse>? QualityChecksSummary = null);
