using Claims.Application.Claims;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using Claims.Controllers.Model;
using Claims.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> _getClaimsHandler;
        private readonly IQueryHandler<GetClaimQuery, GetClaimQueryResult> _getClaimHandler;
        private readonly ICommandHandler<AddClaimCommand, AddClaimCommandResult> _addClaimHandler;
        private readonly IClaimsService _claimsService;
        private readonly Auditer _auditer;

        public ClaimsController(
            IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> getClaimsHandler,
            IQueryHandler<GetClaimQuery, GetClaimQueryResult> getClaimHandler,
            ICommandHandler<AddClaimCommand, AddClaimCommandResult> addClaimHandler,
            IClaimsService claimsService,
            AuditContext auditContext)
        {
            _getClaimsHandler = getClaimsHandler;
            _getClaimHandler = getClaimHandler;
            _addClaimHandler = addClaimHandler;
            _claimsService = claimsService;
            _auditer = new Auditer(auditContext);
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
        public async Task<Claim?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = new GetClaimQuery(id);
            var queryResult = await _getClaimHandler.Handle(query, cancellationToken);
            return queryResult.Claim;
        }

        [HttpPost(Name = "AddClaim")]
        [SwaggerOperation(Summary = "Create new claim")]
        public async Task<ActionResult> CreateAsync(AddClaimDto claim, CancellationToken cancellationToken = default)
        {
            var command = new AddClaimCommand(claim.Name, claim.CoverId, claim.DamageCost, claim.Type);
            var commandResult = await _addClaimHandler.Handle(command, cancellationToken);
            return Ok(commandResult.Claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _auditer.AuditClaim(id, "DELETE");
            return _claimsService.DeleteClaimAsync(id, cancellationToken);
        }
    }
}