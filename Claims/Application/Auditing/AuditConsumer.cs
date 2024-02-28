using Claims.Model;
using MassTransit;

namespace Claims.Application.Auditing;

public record AuditMessage(AuditTypes Type, Guid Id, AuditHttpRequestType RequestType);

public class AuditConsumer(IServiceScopeFactory serviceScopeFactory, ILogger<AuditConsumer> logger)
    : IConsumer<AuditMessage>
{
    public Task Consume(ConsumeContext<AuditMessage> context)
    {
        var message = context.Message;
        switch (message.Type)
        {
            case AuditTypes.Claim:
                AuditClaim(message.Id, message.RequestType);
                break;
            case AuditTypes.Cover:
                AuditCover(message.Id, message.RequestType);
                break;
            default:
                logger.LogError($"Invalid AuditMessage received: {message}");
                break;
        }

        return Task.CompletedTask;
    }

    public void AuditClaim(Guid id, AuditHttpRequestType httpRequestType)
    {
        using var service = serviceScopeFactory.CreateScope();
        var auditContext = service.ServiceProvider.GetRequiredService<AuditContext>();
        var claimAudit = new ClaimAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType.ToString(),
            ClaimId = id.ToString("D")
        };

        auditContext.Add(claimAudit);
        auditContext.SaveChanges();
    }

    public void AuditCover(Guid id, AuditHttpRequestType httpRequestType)
    {
        using var service = serviceScopeFactory.CreateScope();
        var auditContext = service.ServiceProvider.GetRequiredService<AuditContext>();
        var coverAudit = new CoverAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType.ToString(),
            CoverId = id.ToString("D")
        };

        auditContext.Add(coverAudit);
        auditContext.SaveChanges();
    }
}