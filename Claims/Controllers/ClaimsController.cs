using Claims.Application.Claims;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
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
        private readonly IClaimsService _claimsService;
        private readonly Auditer _auditer;

        public ClaimsController(
            IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> getClaimsHandler,
            IQueryHandler<GetClaimQuery, GetClaimQueryResult> getClaimHandler,
            IClaimsService claimsService,
            AuditContext auditContext)
        {
            _getClaimsHandler = getClaimsHandler;
            _getClaimHandler = getClaimHandler;
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

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _claimsService.AddClaimAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            _auditer.AuditClaim(id, "DELETE");
            return _claimsService.DeleteClaimAsync(id, cancellationToken);
        }
    }
}