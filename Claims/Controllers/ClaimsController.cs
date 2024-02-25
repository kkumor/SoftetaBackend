using Claims.Application;
using Claims.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimsService _claimsService;
        private readonly Auditer _auditer;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService,
            AuditContext auditContext)
        {
            _logger = logger;
            _claimsService = claimsService;
            _auditer = new Auditer(auditContext);
        }

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _claimsService.GetClaimsAsync(cancellationToken);
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