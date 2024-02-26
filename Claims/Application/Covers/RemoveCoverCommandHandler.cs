using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using JetBrains.Annotations;

namespace Claims.Application.Covers;

[UsedImplicitly]
public class RemoveCoverCommandHandler(ICoversService coversService, IAuditerService auditer)
    : ICommandHandler<RemoveCoverCommand, RemoveCoverCommandResult>
{
    public async Task<RemoveCoverCommandResult> Handle(RemoveCoverCommand command,
        CancellationToken cancellationToken = default)
    {
        await auditer.Audit(AuditTypes.Cover, command.Id, AuditHttpRequestType.DELETE);
        await coversService.DeleteCoverAsync(command.Id, cancellationToken);
        return new RemoveCoverCommandResult();
    }
}

public record RemoveCoverCommand(Guid Id) : ICommand;

public record RemoveCoverCommandResult() : ICommandResult;