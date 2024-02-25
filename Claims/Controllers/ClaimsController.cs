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
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimsService _claimsService;
        private readonly Auditer _auditer;

        public ClaimsController(IQueryHandler<GetClaimsQuery, GetClaimsQueryResult> getClaimsHandler,
            ILogger<ClaimsController> logger, IClaimsService claimsService,
            AuditContext auditContext)
        {
            _getClaimsHandler = getClaimsHandler;
            _logger = logger;
            _claimsService = claimsService;
            _auditer = new Auditer(auditContext);
        }

        [HttpGet(Name = "GetAllClaims")]
        [SwaggerOperation(Summary = "Return claims collection")]
        public async Task<IEnumerable<Claim>> GetAsync(CancellationToken cancellationToken = default)
        {
            var queryResult = await _getClaimsHandler.Handle(new GetClaimsQuery(), cancellationToken);
            return queryResult.Claims;
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

        [HttpGet("{id}")]
        public Task<Claim?> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return _claimsService.GetClaimAsync(id, cancellationToken);
        }
    }
}