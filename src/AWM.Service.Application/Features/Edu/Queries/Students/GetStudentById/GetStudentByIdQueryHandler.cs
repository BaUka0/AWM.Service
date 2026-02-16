namespace AWM.Service.Application.Features.Edu.Queries.Students.GetStudentById;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;

    public GetStudentByIdQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null || student.IsDeleted)
                return Result.Failure<StudentDto>(new Error("404", $"Student with ID {request.StudentId} not found."));

            var user = await _userRepository.GetByIdAsync(student.UserId, cancellationToken);

            var dto = new StudentDto
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
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<StudentDto>(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
