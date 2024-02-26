using Claims.Application.Services;
using Claims.Application.Shared;
using Claims.Auditing;
using Claims.Model;
using JetBrains.Annotations;

namespace Claims.Application.Covers;

[UsedImplicitly]
public class AddCoverCommandHandler(ICoversService coversService, IRateService rateService, IAuditerService auditer)
    : ICommandHandler<AddCoverCommand, AddCoverCommandResult>
{
    public async Task<AddCoverCommandResult> Handle(AddCoverCommand command,
        CancellationToken cancellationToken = default)
    {
        var coverId = Guid.NewGuid();
        var cover = new Cover
        {
            Id = coverId.ToString("D"),
            Type = command.Type,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Premium = rateService.ComputePremium(command.StartDate, command.EndDate, command.Type)
        };
        await coversService.AddCoverAsync(cover, cancellationToken);
        await auditer.Audit(AuditTypes.Cover, coverId, AuditHttpRequestType.POST);
        return new AddCoverCommandResult(cover);
    }
}

public record AddCoverCommand(CoverType Type, DateOnly StartDate, DateOnly EndDate) : ICommand;

public record AddCoverCommandResult(Cover Cover) : ICommandResult;