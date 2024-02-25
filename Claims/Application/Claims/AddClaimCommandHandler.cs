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
    public async Task<AddClaimCommandResult> Handle(AddClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        var claimId = Guid.NewGuid();
        var claim = new Claim
        {
            Id = claimId.ToString("D"),
            Name = command.Name,
            CoverId = command.CoverId,
            Type = command.Type,
            DamageCost = command.DamageCost,
            Created = DateTime.UtcNow
        };
        await claimsService.AddClaimAsync(claim, cancellationToken);
        auditer.AuditClaim(claimId, "POST");
        return new AddClaimCommandResult(claim);
    }
}

public record AddClaimCommand(string Name, string CoverId, decimal DamageCost, ClaimType Type) : ICommand;

public record AddClaimCommandResult(Claim Claim) : ICommandResult;