using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Covers;

[UsedImplicitly]
public class GetCoversQueryHandler(ICoversService coversService) : IQueryHandler<GetCoversQuery, GetCoversQueryResult>
{
    public async Task<GetCoversQueryResult> Handle(GetCoversQuery query, CancellationToken cancellationToken = default)
    {
        var covers = await coversService.GetCoversAsync(cancellationToken);
        return new GetCoversQueryResult(covers);
    }
}

public record GetCoversQuery() : IQuery;

public record GetCoversQueryResult(IEnumerable<Cover> Covers) : IQueryResult;