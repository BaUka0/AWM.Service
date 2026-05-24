namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetUserAccessMatrix;

using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

public sealed record GetUserAccessMatrixQuery : IRequest<IReadOnlyList<UserAccessMatrix>>
{
    public int UserId { get; init; }
}
