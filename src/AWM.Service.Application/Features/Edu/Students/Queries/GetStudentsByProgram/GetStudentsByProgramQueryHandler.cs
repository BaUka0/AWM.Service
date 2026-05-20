namespace AWM.Service.Application.Features.Edu.Students.Queries.GetStudentsByProgram;

using AWM.Service.Application.Features.Edu.Students.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetStudentsByProgramQueryHandler : IRequestHandler<GetStudentsByProgramQuery, Result<IReadOnlyList<StudentDto>>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISpecialityRepository _SpecialityRepository;

    public GetStudentsByProgramQueryHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ISpecialityRepository SpecialityRepository)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _SpecialityRepository = SpecialityRepository ?? throw new ArgumentNullException(nameof(SpecialityRepository));
    }

    public async Task<Result<IReadOnlyList<StudentDto>>> Handle(GetStudentsByProgramQuery request, CancellationToken cancellationToken)
    {
        var speciality = await _SpecialityRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        var programName = speciality?.Title ?? string.Empty;

        var students = await _studentRepository.GetBySpecialityAsync(request.ProgramId, cancellationToken);
        var userIds = students.Select(s => s.Id).Distinct().ToList();
        var users = (await _userRepository.GetByIdsAsync(userIds, cancellationToken)).ToDictionary(u => u.Id);

        var dtos = students.Select(s =>
        {
            users.TryGetValue(s.Id, out var user);
            return new StudentDto
            {
                Id = s.Id,
                UserId = s.Id,
                FullName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : string.Empty,
                Email = user?.Email,
                GroupCode = string.Empty, // Or resolve from Student group if available in University DB
                ProgramId = request.ProgramId,
                ProgramName = programName,
                AdmissionYear = s.Year,
                CurrentCourse = DateTime.UtcNow.Year - s.Year, // Simplified course calculation
                Status = s.StatusId == 1 ? "Active" : "Inactive", // Depending on StudentStatus lookup
                CreatedAt = default,
                CreatedBy = 0
            };
        }).ToList();

        return Result.Success<IReadOnlyList<StudentDto>>(dtos);
    }
}
