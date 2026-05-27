using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SaveRepoUrl;

public record SaveRepoUrlCommand(long WorkId, string RepoUrl) : IRequest<Result>;

public sealed class SaveRepoUrlCommandHandler : IRequestHandler<SaveRepoUrlCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SaveRepoUrlCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SaveRepoUrlCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        // Only participants can save their repo URL
        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        if (!isParticipant)
        {
            return Result.Failure(new Error("StudentWorks.Forbidden", "Only participants of this work can update the repository URL."));
        }

        // Parse existing MetadataJson (or start fresh) and set softwareCheckRepoUrl
        JsonObject metadata;
        if (!string.IsNullOrWhiteSpace(work.MetadataJson))
        {
            try
            {
                metadata = JsonNode.Parse(work.MetadataJson)?.AsObject() ?? new JsonObject();
            }
            catch
            {
                metadata = new JsonObject();
            }
        }
        else
        {
            metadata = new JsonObject();
        }

        metadata["softwareCheckRepoUrl"] = request.RepoUrl;

        work.UpdateMetadata(metadata.ToJsonString(), currentUserId);

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
