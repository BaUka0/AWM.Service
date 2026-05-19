namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using AWM.Service.Application.Features.Auth.DTOs;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetCurrentUserProfileQuery.
/// Returns full profile data for the authenticated user.
/// </summary>
public sealed class GetCurrentUserProfileQueryHandler
    : IRequestHandler<GetCurrentUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetCurrentUserProfileQueryHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IStaffRepository staffRepository,
        IReviewerRepository reviewerRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _staffRepository = staffRepository;
        _reviewerRepository = reviewerRepository;
        _orgLookupRepository = orgLookupRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        return Result.Failure<UserProfileDto>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
