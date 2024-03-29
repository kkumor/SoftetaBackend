using Claims.Api.Controllers.Model;
using Claims.Application.Claims;
using Claims.Application.Shared;
using Claims.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> _getClaimsHandler;
        private readonly IQueryHandler<GetClaimQuery, GetClaimQueryResult> _getClaimHandler;
        private readonly ICommandHandler<AddClaimCommand, AddClaimCommandResult> _addClaimHandler;
        private readonly IValidator<AddClaimDto> _addClaimValidator;
        private readonly ICommandHandler<RemoveClaimCommand, RemoveClaimCommandResult> _removeClaimHandler;

        public ClaimsController(
            IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> getClaimsHandler,
            IQueryHandler<GetClaimQuery, GetClaimQueryResult> getClaimHandler,
            ICommandHandler<AddClaimCommand, AddClaimCommandResult> addClaimHandler,
            IValidator<AddClaimDto> addClaimValidator,
            ICommandHandler<RemoveClaimCommand, RemoveClaimCommandResult> removeClaimHandler)
        {
            _getClaimsHandler = getClaimsHandler;
            _getClaimHandler = getClaimHandler;
            _addClaimHandler = addClaimHandler;
            _addClaimValidator = addClaimValidator;
            _removeClaimHandler = removeClaimHandler;
        }

        [HttpGet(Name = "GetAllClaims")]
        [SwaggerOperation(Summary = "Get claims collection")]
        public async Task<IEnumerable<Claim>> GetAsync(CancellationToken cancellationToken = default)
        {
            var queryResult = await _getClaimsHandler.Handle(new GetClaimsQuery(), cancellationToken);
            return queryResult.Claims;
        }

        [HttpGet("{id}", Name = "GetSingleClaim")]
        [SwaggerOperation(Summary = "Get claim by id")]
        public async Task<ActionResult<Claim?>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetClaimQuery(id);
            var queryResult = await _getClaimHandler.Handle(query, cancellationToken);
            return queryResult.Claim != default ? Ok(queryResult.Claim) : NotFound();
        }

        [HttpPost(Name = "AddClaim")]
        [SwaggerOperation(Summary = "Create new claim")]
        public async Task<ActionResult> CreateAsync(AddClaimDto claim, CancellationToken cancellationToken = default)
        {
            var validatorResult = await _addClaimValidator.Validate(claim);
            if (!validatorResult.IsValid)
            {
                return BadRequest(validatorResult.Errors);
            }

            var command = new AddClaimCommand(claim.Name, claim.CoverId, claim.DamageCost, claim.Type, claim.Created);
            var commandResult = await _addClaimHandler.Handle(command, cancellationToken);
            return Ok(commandResult.Claim);
        }

        [HttpDelete("{id}", Name = "RemoveClaim")]
        [SwaggerOperation(Summary = "Remove claim")]
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var command = new RemoveClaimCommand(id);
            await _removeClaimHandler.Handle(command, cancellationToken);
        }
    }
}