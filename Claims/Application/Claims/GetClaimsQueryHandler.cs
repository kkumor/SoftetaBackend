using Claims.Application.Services;
using Claims.Application.Shared;
using JetBrains.Annotations;

namespace Claims.Application.Claims;

[UsedImplicitly]
public class GetClaimsQueryHandler(IClaimsService claimsService) : IQueryHandler<GetClaimsQuery, GetClaimsQueryResult>
{
    public async Task<GetClaimsQueryResult> Handle(GetClaimsQuery query, CancellationToken cancellationToken = default)
    {
        var claims = await claimsService.GetClaimsAsync(cancellationToken);
        return new GetClaimsQueryResult(claims);
    }
}

public record GetClaimsQuery;

public record GetClaimsQueryResult(IEnumerable<Claim> Claims);