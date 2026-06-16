namespace AWM.Service.Application.Features.University.Queries.GetStudents;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Result<IReadOnlyList<StudentDto>>>
{
    private readonly IStudentReadOnlyRepository _studentRepo;
    public GetStudentsQueryHandler(IStudentReadOnlyRepository studentRepo) { _studentRepo = studentRepo; }
    public async Task<Result<IReadOnlyList<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _studentRepo.GetAllAsync(cancellationToken);

        var query = students.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(x => x.User != null && ($" {x.User.LastName} {x.User.FirstName}".ToLower().Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
        }

        var dtos = query.Select(s => new StudentDto(
            s.Id,
            s.User != null ? $"{s.User.LastName} {s.User.FirstName} {s.User.MiddleName}".Trim() : "Unknown",
            s.Speciality != null ? $"{s.Speciality.Code}-{s.Year}" : "CS-401",
            s.Speciality?.Title ?? "Unknown",
            s.Year,
            s.Status?.Title?.ToLower() ?? "active"
        )).ToList();

        return Result.Success<IReadOnlyList<StudentDto>>(dtos);
    }
}
