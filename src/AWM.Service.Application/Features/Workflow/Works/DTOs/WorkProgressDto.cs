using System;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Works.DTOs;

public record MultilingualTextDto(string Ru, string Kk, string En);

public record WorkParticipantDto(int StudentId, string Name, string Role);

public record WorkAttachmentDto(
    long Id,
    int? StateId,
    int AttachmentTypeId,
    string FileName,
    string FileStoragePath,
    long FileSizeBytes,
    string ContentType,
    DateTime CreatedAt,
    string UploadedBy);

public record WorkProgressDto(
    long WorkId,
    int SemesterId,
    int OrgUnitId,
    int? SpecialityId,
    int CurrentStateId,
    string CurrentStateName,
    string CurrentStateDisplayName,
    long? TopicId,
    MultilingualTextDto TopicTitle,
    MultilingualTextDto DirectionTitle,
    string SupervisorName,
    string SupervisorContacts,
    DateTime CreatedAt,
    string WorkTypeName,
    bool IsDefended,
    string FinalGrade,
    IReadOnlyList<WorkParticipantDto> Participants,
    IReadOnlyList<WorkAttachmentDto> Attachments,
    IReadOnlyList<string> Timeline);
