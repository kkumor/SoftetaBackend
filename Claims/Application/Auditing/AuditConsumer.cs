using Claims.Model;
using MassTransit;

namespace Claims.Application.Auditing;

public record AuditMessage(AuditTypes Type, Guid Id, AuditHttpRequestType RequestType);

public class AuditConsumer(IServiceScopeFactory serviceScopeFactory, ILogger<AuditConsumer> logger)
    : IConsumer<AuditMessage>
{
    public async Task Consume(ConsumeContext<AuditMessage> context)
    {
        var message = context.Message;
        switch (message.Type)
        {
            case AuditTypes.Claim:
                await AuditClaim(message.Id, message.RequestType);
                return;
            case AuditTypes.Cover:
                await AuditCover(message.Id, message.RequestType);
                return;
            default:
                logger.LogError($"Invalid AuditMessage received: {message}");
                break;
        }
    }

    public async Task AuditClaim(Guid id, AuditHttpRequestType httpRequestType)
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
        await auditContext.SaveChangesAsync();
    }

    public async Task AuditCover(Guid id, AuditHttpRequestType httpRequestType)
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
        await auditContext.SaveChangesAsync();
    }
}