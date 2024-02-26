using Claims.Application.Auditing;
using Claims.Application.Covers;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Controllers.Model;
using Claims.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICommandHandler<AddCoverCommand, AddCoverCommandResult> _addCoverCommandHandler;
    private readonly ICommandHandler<RemoveCoverCommand, RemoveCoverCommandResult> _removeCoverCommandHandler;
    private readonly IRateService _rateService;
    private readonly Auditer _auditer;
    private readonly Container _container;

    public CoversController(
        ICommandHandler<AddCoverCommand, AddCoverCommandResult> addCoverCommandHandler,
        ICommandHandler<RemoveCoverCommand, RemoveCoverCommandResult> removeCoverCommandHandler,
        IRateService rateService,
        CosmosClient cosmosClient, Auditer auditer)
    {
        _addCoverCommandHandler = addCoverCommandHandler;
        _removeCoverCommandHandler = removeCoverCommandHandler;
        _rateService = rateService;
        _auditer = auditer;
        _container = cosmosClient.GetContainer("ClaimDb", "Cover") ??
                     throw new ArgumentNullException(nameof(cosmosClient));
    }

    [HttpPost("actions/compute-premium", Name = "Compute Premium")]
    [SwaggerOperation(Summary = "Compute Premium for given parameters")]
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

    [HttpPost(Name = "AddCover")]
    [SwaggerOperation(Summary = "Create new cover")]
    public async Task<ActionResult> CreateAsync(AddCoverDto cover, CancellationToken cancellationToken = default)
    {
        var addCoverCommand = new AddCoverCommand(cover.Type, cover.StartDate, cover.EndDate);
        var addCoverResult = await _addCoverCommandHandler.Handle(addCoverCommand, cancellationToken);
        return Ok(addCoverResult.Cover);
    }

    [HttpDelete("{id}", Name = "RemoveCover")]
    [SwaggerOperation(Summary = "Remove cover")]
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var command = new RemoveCoverCommand(id);
        await _removeCoverCommandHandler.Handle(command, cancellationToken);
    }
}