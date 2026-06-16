using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Works;

public sealed record MarkAsGraduatedRequest(
    IReadOnlyList<long> WorkIds
);
