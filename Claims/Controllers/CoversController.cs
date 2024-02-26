using Claims.Application.Covers;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using Claims.Controllers.Model;
using Claims.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICommandHandler<AddCoverCommand, AddCoverCommandResult> _addCoverCommandHandler;
    private readonly IRateService _rateService;
    private readonly Auditer _auditer;
    private readonly Container _container;

    public CoversController(
        ICommandHandler<AddCoverCommand, AddCoverCommandResult> addCoverCommandHandler,
        IRateService rateService,
        CosmosClient cosmosClient, Auditer auditer)
    {
        _addCoverCommandHandler = addCoverCommandHandler;
        _rateService = rateService;
        _auditer = auditer;
        _container = cosmosClient.GetContainer("ClaimDb", "Cover") ??
                     throw new ArgumentNullException(nameof(cosmosClient));
    }

    [HttpPost("actions/compute-premium")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var computePremium = _rateService.ComputePremium(startDate, endDate, coverType);
        return Ok(computePremium);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var query = _container.GetItemQueryIterator<Cover>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<Cover>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Cover>(id, new(id));
            return Ok(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(AddCoverDto cover, CancellationToken cancellationToken = default)
    {
        var addCoverCommand = new AddCoverCommand(cover.Type, cover.StartDate, cover.EndDate);
        var addCoverResult = await _addCoverCommandHandler.Handle(addCoverCommand, cancellationToken);
        return Ok(addCoverResult.Cover);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(string id)
    {
        _auditer.AuditCover(new Guid(id), "DELETE");
        return _container.DeleteItemAsync<Cover>(id, new(id));
    }
}