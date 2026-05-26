using System;
using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Works;

public record MultilingualTextResponse(string Ru, string Kk, string En);

public record WorkParticipantResponse(int StudentId, string Name, string Role);

public record WorkAttachmentResponse(
    long Id,
    int? StateId,
    int AttachmentTypeId,
    string FileName,
    string FileStoragePath,
    long FileSizeBytes,
    string ContentType,
    DateTime CreatedAt,
    string UploadedBy);

public record WorkProgressResponse(
    long WorkId,
    int SemesterId,
    int OrgUnitId,
    int? SpecialityId,
    int CurrentStateId,
    string CurrentStateName,
    string CurrentStateDisplayName,
    long? TopicId,
    MultilingualTextResponse TopicTitle,
    MultilingualTextResponse DirectionTitle,
    string SupervisorName,
    string SupervisorContacts,
    DateTime CreatedAt,
    string WorkTypeName,
    bool IsDefended,
    string FinalGrade,
    IReadOnlyList<WorkParticipantResponse> Participants,
    IReadOnlyList<WorkAttachmentResponse> Attachments,
    IReadOnlyList<string> Timeline);
