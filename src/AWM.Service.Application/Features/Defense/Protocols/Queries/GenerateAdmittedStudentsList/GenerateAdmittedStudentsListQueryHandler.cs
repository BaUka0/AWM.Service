using AWM.Service.Application.Features.Workflow.Works.Queries.GetDefenseReadiness;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateAdmittedStudentsList;

public sealed class GenerateAdmittedStudentsListQueryHandler
    : IRequestHandler<GenerateAdmittedStudentsListQuery, Result<byte[]>>
{
    private readonly ISender _sender;
    private readonly IOrgUnitReadOnlyRepository _orgUnitRepository;
    private readonly ISemesterReadOnlyRepository _semesterRepository;
    private readonly IPdfReportService _pdfReportService;

    public GenerateAdmittedStudentsListQueryHandler(
        ISender sender,
        IOrgUnitReadOnlyRepository orgUnitRepository,
        ISemesterReadOnlyRepository semesterRepository,
        IPdfReportService pdfReportService)
    {
        _sender = sender;
        _orgUnitRepository = orgUnitRepository;
        _semesterRepository = semesterRepository;
        _pdfReportService = pdfReportService;
    }

    public async Task<Result<byte[]>> Handle(
        GenerateAdmittedStudentsListQuery request,
        CancellationToken cancellationToken)
    {
        var readinessResult = await _sender.Send(
            new GetDefenseReadinessQuery(request.OrgUnitId, request.SemesterId),
            cancellationToken);

        if (readinessResult.IsFailed)
            return Result.Failure<byte[]>(readinessResult.Error);

        var admittedStudents = readinessResult.Value
            .Where(d => d.Admitted)
            .OrderBy(d => d.StudentName)
            .ToList();

        var orgUnit = await _orgUnitRepository.GetByIdAsync(request.OrgUnitId, cancellationToken);
        var semester = await _semesterRepository.GetByIdAsync(request.SemesterId, cancellationToken);

        var reportData = new AdmittedStudentsListData(
            orgUnit?.Title ?? $"Кафедра #{request.OrgUnitId}",
            semester?.Title ?? $"Семестр #{request.SemesterId}",
            DateTime.UtcNow.ToString("dd.MM.yyyy"),
            admittedStudents
                .Select((dto, idx) => new AdmittedStudentData(
                    idx + 1,
                    dto.StudentName,
                    dto.TopicTitle,
                    "—"))
                .ToList());

        var pdfBytes = await _pdfReportService.GenerateAdmittedStudentsListAsync(reportData);
        return Result.Success(pdfBytes);
    }
}
