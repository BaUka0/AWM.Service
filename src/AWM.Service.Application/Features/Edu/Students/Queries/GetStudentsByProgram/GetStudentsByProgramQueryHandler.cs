namespace AWM.Service.Application.Features.Edu.Students.Queries.GetStudentsByProgram;

using AWM.Service.Application.Features.Edu.Students.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStudentsByProgramQueryHandler : IRequestHandler<GetStudentsByProgramQuery, Result<IReadOnlyList<StudentDto>>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;

    public GetStudentsByProgramQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<StudentDto>>> Handle(GetStudentsByProgramQuery request, CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<StudentDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
