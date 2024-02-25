using System.Globalization;
using Claims.Application;
using Claims.Application.Services;
using Microsoft.Azure.Cosmos;

namespace Claims.Infrastructure;

public class ClaimsService(Container container) : IClaimsService
{
    public async Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
    {
        var claimsQueryDefinition = new QueryDefinition("SELECT * FROM c");
        var queryResult = container.GetItemQueryIterator<Claim>(claimsQueryDefinition);
        var results = new List<Claim>();
        while (queryResult.HasMoreResults)
        {
            var response = await queryResult.ReadNextAsync(cancellationToken);
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<Claim?> GetClaimAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerId = id.ToString("D");
            var response = await container.ReadItemAsync<Claim>(containerId, new PartitionKey(containerId), null,
                cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    public Task AddClaimAsync(Claim item, CancellationToken cancellationToken = default) =>
        container.CreateItemAsync(item, new PartitionKey(item.Id), null, cancellationToken);

    public Task DeleteClaimAsync(string id, CancellationToken cancellationToken = default) =>
        container.DeleteItemAsync<Claim>(id, new PartitionKey(id), null, cancellationToken);
}