namespace AWM.Service.Application.Features.Edu.Commands.Students.CreateStudent;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<int>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result.Failure<int>(new Error(DomainErrors.Auth.InvalidCredentials, $"User with ID {request.UserId} not found."));

            var existingStudent = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingStudent is not null && !existingStudent.IsDeleted)
                return Result.Failure<int>(new Error(DomainErrors.Edu.Student.AlreadyExists, $"Student profile for user {request.UserId} already exists."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error(DomainErrors.Auth.InvalidCredentials, "User ID is not available."));

            var student = new Student(
                request.UserId,
                request.ProgramId,
                request.AdmissionYear,
                request.CurrentCourse,
                userId.Value,
                request.GroupCode);

            await _studentRepository.AddAsync(student, cancellationToken);
            return Result.Success(student.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error(DomainErrors.Edu.Student.GenericError, argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error(DomainErrors.General.InternalError, $"An error occurred while creating the Student: {ex.Message}"));
        }
    }
}
