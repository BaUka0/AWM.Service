using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;

public sealed record GetCommissionByIdQuery(int Id) : IRequest<Result<AWM.Service.Application.Features.Defense.Commissions.DTOs.CommissionDto>>;
