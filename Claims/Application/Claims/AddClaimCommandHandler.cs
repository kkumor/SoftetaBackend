using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using Claims.Infrastructure;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Claims;

[UsedImplicitly]
public class AddClaimCommandHandler(IClaimsService claimsService, Auditer auditer)
    : ICommandHandler<AddClaimCommand, AddClaimCommandResult>
{
    public async Task<AddClaimCommandResult> Handle(AddClaimCommand query,
        CancellationToken cancellationToken = default)
    {
        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            Name = query.Name,
            CoverId = query.CoverId,
            Type = query.Type,
            DamageCost = query.DamageCost,
            Created = DateTime.UtcNow
        };
        await claimsService.AddClaimAsync(claim, cancellationToken);
        auditer.AuditClaim(claim.Id, "POST");
        return new AddClaimCommandResult(claim);
    }
}

public record AddClaimCommand(string Name, string CoverId, decimal DamageCost, ClaimType Type) : ICommand;

public record AddClaimCommandResult(Claim Claim) : ICommandResult;