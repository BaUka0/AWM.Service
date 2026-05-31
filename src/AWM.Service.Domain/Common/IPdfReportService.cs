using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Domain.Common;

public sealed record MemberGradeData(
    string MemberName,
    string MemberRole,
    string CriteriaName,
    int Score,
    string? Comment
);

public sealed record ProtocolReportData(
    long ProtocolId,
    string? ProtocolNumber,
    string CommissionName,
    string CommissionType, // "PreDefense" or "GAK"
    string SessionDate,
    string StudentName,
    string TopicTitle,
    string SpecialityName,
    decimal FinalScore,
    string FinalGradeLetter,
    string Decision,
    string? Comments,
    IReadOnlyList<MemberGradeData> Grades
);

public sealed record AdmittedStudentData(
    int Number,
    string StudentName,
    string TopicTitle,
    string SupervisorName
);

public sealed record AdmittedStudentsListData(
    string OrgUnitName,
    string SemesterName,
    string GeneratedDate,
    IReadOnlyList<AdmittedStudentData> Students
);

public sealed record ScheduleReportItem(
    string Date,
    string StartTime,
    string StudentName,
    string TopicTitle,
    string Location
);

public sealed record ScheduleReportData(
    string CommissionName,
    string GeneratedDate,
    IReadOnlyList<ScheduleReportItem> Items
);

public interface IPdfReportService
{
    Task<byte[]> GenerateProtocolReportAsync(ProtocolReportData data);
    Task<byte[]> GenerateAdmittedStudentsListAsync(AdmittedStudentsListData data);
    Task<byte[]> GenerateScheduleReportAsync(ScheduleReportData data);
}

