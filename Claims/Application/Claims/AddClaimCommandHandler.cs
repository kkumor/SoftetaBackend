using Claims.Application.Auditing;
using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Claims;

[UsedImplicitly]
public class AddClaimCommandHandler(IClaimsService claimsService, IAuditerService auditer)
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
            CoverId = command.CoverId.ToString("D"),
            Type = command.Type,
            DamageCost = command.DamageCost,
            Created = command.Created
        };
        await claimsService.AddClaimAsync(claim, cancellationToken);
        await auditer.Audit(AuditTypes.Claim, claimId, AuditHttpRequestType.POST);
        return new AddClaimCommandResult(claim);
    }
}

public record AddClaimCommand(string Name, Guid CoverId, decimal DamageCost, ClaimType Type, DateTime Created)
    : ICommand;

public record AddClaimCommandResult(Claim Claim) : ICommandResult;