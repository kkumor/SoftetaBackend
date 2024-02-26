using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using JetBrains.Annotations;

namespace Claims.Application.Claims;

[UsedImplicitly]
public class RemoveClaimCommandHandler(IClaimsService claimsService, IAuditerService auditer)
    : ICommandHandler<RemoveClaimCommand, RemoveClaimCommandResult>
{
    public async Task<RemoveClaimCommandResult> Handle(RemoveClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        await auditer.Audit(AuditTypes.Claim, command.Id, AuditHttpRequestType.DELETE);
        await claimsService.DeleteClaimAsync(command.Id, cancellationToken);
        return new RemoveClaimCommandResult();
    }
}

public record RemoveClaimCommand(Guid Id) : ICommand;

public record RemoveClaimCommandResult() : ICommandResult;