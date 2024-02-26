using Claims.Application.Covers;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Controllers.Model;
using Claims.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using GetCoversQuery = Claims.Application.Covers.GetCoversQuery;

namespace Claims.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly IQueryHandler<GetCoversQuery, GetCoversQueryResult> _getCoversHandler;
    private readonly IQueryHandler<GetCoverQuery, GetCoverQueryResult> _getCoverHandler;
    private readonly ICommandHandler<AddCoverCommand, AddCoverCommandResult> _addCoverCommandHandler;
    private readonly ICommandHandler<RemoveCoverCommand, RemoveCoverCommandResult> _removeCoverCommandHandler;
    private readonly IRateService _rateService;

    public CoversController(
        IQueryHandler<GetCoversQuery, GetCoversQueryResult> getCoversHandler,
        IQueryHandler<GetCoverQuery, GetCoverQueryResult> getCoverHandler,
        ICommandHandler<AddCoverCommand, AddCoverCommandResult> addCoverCommandHandler,
        ICommandHandler<RemoveCoverCommand, RemoveCoverCommandResult> removeCoverCommandHandler,
        IRateService rateService)
    {
        _getCoversHandler = getCoversHandler;
        _getCoverHandler = getCoverHandler;
        _addCoverCommandHandler = addCoverCommandHandler;
        _removeCoverCommandHandler = removeCoverCommandHandler;
        _rateService = rateService;
    }

    [HttpPost("actions/compute-premium", Name = "Compute Premium")]
    [SwaggerOperation(Summary = "Compute Premium for given parameters")]
    public async Task<ActionResult> ComputePremiumAsync(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var computePremium = _rateService.ComputePremium(startDate, endDate, coverType);
        return Ok(computePremium);
    }

    [HttpGet(Name = "GetAllCovers")]
    [SwaggerOperation(Summary = "Get covers collection")]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync(CancellationToken cancellationToken = default)
    {
        var queryResult = await _getCoversHandler.Handle(new GetCoversQuery(), cancellationToken);
        return new OkObjectResult(queryResult.Covers);
    }

    [HttpGet("{id}", Name = "GetSingleCover")]
    [SwaggerOperation(Summary = "Get cover by id")]
    public async Task<ActionResult<Cover?>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = new GetCoverQuery(id);
        var queryResult = await _getCoverHandler.Handle(query, cancellationToken);
        return queryResult.Cover != default ? Ok(queryResult.Cover) : NotFound();
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