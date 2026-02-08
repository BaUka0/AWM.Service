namespace AWM.Service.Application.Features.Edu.Commands.Students.UpdateStudent;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateStudentCommandHandler(
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null || student.IsDeleted)
                return Result.Failure(new Error(DomainErrors.Edu.Student.NotFound, $"Student with ID {request.StudentId} not found."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error(DomainErrors.Auth.InvalidCredentials, "User ID is not available."));

            if (request.GroupCode is not null)
                student.UpdateGroup(request.GroupCode, userId.Value);

            if (request.CurrentCourse.HasValue)
                student.PromoteToCourse(request.CurrentCourse.Value, userId.Value);

            await _studentRepository.UpdateAsync(student, cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error(DomainErrors.Edu.Student.GenericError, argEx.Message));
        }
        catch (InvalidOperationException opEx)
        {
            return Result.Failure(new Error(DomainErrors.Edu.Student.GenericError, opEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(DomainErrors.General.InternalError, $"An error occurred while updating the Student: {ex.Message}"));
        }
    }
}
