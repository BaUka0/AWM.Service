namespace AWM.Service.Application.Features.Edu.Students.Commands.CreateStudent;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;
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
        return Result.Failure<int>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
