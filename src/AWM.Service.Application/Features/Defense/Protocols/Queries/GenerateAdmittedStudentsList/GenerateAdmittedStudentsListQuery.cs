using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateAdmittedStudentsList;

public sealed record GenerateAdmittedStudentsListQuery(
    int OrgUnitId,
    int SemesterId) : IRequest<Result<byte[]>>;
