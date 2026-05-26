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

public interface IPdfReportService
{
    Task<byte[]> GenerateProtocolReportAsync(ProtocolReportData data);
}
