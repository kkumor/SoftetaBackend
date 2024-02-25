using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Claims;

[UsedImplicitly]
public class GetClaimQueryHandler(IClaimsService claimsService) : IQueryHandler<GetClaimQuery, GetClaimQueryResult>
{
    public async Task<GetClaimQueryResult> Handle(GetClaimQuery query, CancellationToken cancellationToken = default)
    {
        var claim = await claimsService.GetClaimAsync(query.Id, cancellationToken);
        return new GetClaimQueryResult(claim);
    }
}

public record GetClaimQuery(Guid Id);

public record GetClaimQueryResult(Claim? Claim);