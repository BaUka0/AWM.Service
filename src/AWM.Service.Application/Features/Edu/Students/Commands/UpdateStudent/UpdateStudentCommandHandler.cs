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
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
