using Claims.Application.Services;
using Claims.Model;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure;

public class CoversService(Container container) : ICoversService
{
    public async Task<IEnumerable<Cover>> GetCoversAsync(CancellationToken cancellationToken = default)
    {
        var coversQuery = new QueryDefinition("SELECT * FROM c");
        var queryResult = container.GetItemQueryIterator<Cover>(coversQuery);
        var results = new List<Cover>();
        while (queryResult.HasMoreResults)
        {
            var response = await queryResult.ReadNextAsync(cancellationToken);
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<Cover?> GetCoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerId = id.ToString("D");
            var response = await container.ReadItemAsync<Cover>(containerId, new PartitionKey(containerId), null,
                cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public Task AddCoverAsync(Cover item, CancellationToken cancellationToken = default) =>
        container.CreateItemAsync(item, new PartitionKey(item.Id), cancellationToken: cancellationToken);

    public Task DeleteCoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var containerId = id.ToString("D");
        return container.DeleteItemAsync<Cover>(containerId, new PartitionKey(containerId), null, cancellationToken);
    }
}