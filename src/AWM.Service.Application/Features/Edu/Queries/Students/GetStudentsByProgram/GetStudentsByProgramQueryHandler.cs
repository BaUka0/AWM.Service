namespace AWM.Service.Application.Features.Edu.Queries.Students.GetStudentsByProgram;

using AWM.Service.Application.Features.Edu.DTOs;
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
        try
        {
            var students = await _studentRepository.GetByProgramAsync(request.ProgramId, cancellationToken);

            var dtos = new List<StudentDto>();
            foreach (var student in students.Where(s => !s.IsDeleted))
            {
                var user = await _userRepository.GetByIdAsync(student.UserId, cancellationToken);
                dtos.Add(new StudentDto
                {
                    Id = student.Id,
                    UserId = student.UserId,
                    FullName = user?.Login,
                    Email = user?.Email,
                    GroupCode = student.GroupCode,
                    ProgramId = student.ProgramId,
                    AdmissionYear = student.AdmissionYear,
                    CurrentCourse = student.CurrentCourse,
                    Status = student.Status.ToString(),
                    CreatedAt = student.CreatedAt,
                    CreatedBy = student.CreatedBy,
                    LastModifiedAt = student.LastModifiedAt,
                    LastModifiedBy = student.LastModifiedBy
                });
            }

            return Result.Success<IReadOnlyList<StudentDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<StudentDto>>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
