namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetProtocol;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a protocol by its ID.
/// </summary>
public sealed class GetProtocolQueryHandler
    : IRequestHandler<GetProtocolQuery, Result<ProtocolDto>>
{
    private readonly IProtocolRepository _protocolRepository;

    public GetProtocolQueryHandler(IProtocolRepository protocolRepository)
    {
        _protocolRepository = protocolRepository ?? throw new ArgumentNullException(nameof(protocolRepository));
    }

    public async Task<Result<ProtocolDto>> Handle(
        GetProtocolQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var protocol = await _protocolRepository.GetByIdAsync(request.ProtocolId, cancellationToken);
            if (protocol is null)
                return Result.Failure<ProtocolDto>(new Error("NotFound.Protocol",
                    $"Protocol with ID {request.ProtocolId} not found."));

            var dto = new ProtocolDto
            {
                Id = protocol.Id,
                ScheduleId = protocol.ScheduleId,
                CommissionId = protocol.CommissionId,
                SessionDate = protocol.SessionDate,
                DocumentPath = protocol.DocumentPath,
                IsFinalized = protocol.IsFinalized,
                FinalizedBy = protocol.FinalizedBy,
                FinalizedAt = protocol.FinalizedAt,
                CreatedAt = protocol.CreatedAt
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ProtocolDto>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}
