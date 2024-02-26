using Claims.Model;

namespace Claims.Application.Services;

public interface ICoversService
{
    Task<IEnumerable<Cover>> GetCoversAsync(CancellationToken cancellationToken = default);
    Task<Cover?> GetCoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddCoverAsync(Cover item, CancellationToken cancellationToken = default);
    Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default);
}