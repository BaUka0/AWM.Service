using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

public record DeleteCommissionCommand(int Id) : IRequest<Result>;
