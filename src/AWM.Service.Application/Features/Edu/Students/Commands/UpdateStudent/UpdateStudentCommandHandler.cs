namespace AWM.Service.Application.Features.Edu.Students.Commands.UpdateStudent;

using AWM.Service.Domain.Common;
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
                return Result.Failure(new Error("404", $"Student with ID {request.StudentId} not found."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            if (request.GroupCode is not null)
                student.UpdateGroup(request.GroupCode, userId.Value);

            if (request.CurrentCourse.HasValue)
                student.PromoteToCourse(request.CurrentCourse.Value, userId.Value);

            await _studentRepository.UpdateAsync(student, cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (InvalidOperationException opEx)
        {
            return Result.Failure(new Error("400", opEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while updating the Student: {ex.Message}"));
        }
    }
}
