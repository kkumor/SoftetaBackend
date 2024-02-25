using Claims.Model;

namespace Claims.Application.Services;

public interface IClaimsService
{
    Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default);
    Task<Claim?> GetClaimAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddClaimAsync(Claim item, CancellationToken cancellationToken = default);
    Task DeleteClaimAsync(Guid id, CancellationToken cancellationToken = default);
}