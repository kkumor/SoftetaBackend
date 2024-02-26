using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Covers;

[UsedImplicitly]
public class GetCoverQueryHandler(ICoversService coversService) : IQueryHandler<GetCoverQuery, GetCoverQueryResult>
{
    public async Task<GetCoverQueryResult> Handle(GetCoverQuery query, CancellationToken cancellationToken = default)
    {
        var cover = await coversService.GetCoverAsync(query.Id, cancellationToken);
        return new GetCoverQueryResult(cover);
    }
}

public record GetCoverQuery(Guid Id) : IQuery;

public record GetCoverQueryResult(Cover? Cover) : IQueryResult;