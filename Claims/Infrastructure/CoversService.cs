using Claims.Application.Services;
using Claims.Model;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure;

public class CoversService(Container container) : ICoversService
{
    public Task<IEnumerable<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Cover?> GetCoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddCoverAsync(Cover item, CancellationToken cancellationToken = default) =>
        container.CreateItemAsync(item, new PartitionKey(item.Id), cancellationToken: cancellationToken);

    public Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}