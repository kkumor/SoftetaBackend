using Claims.Model;
using MassTransit;

namespace Claims.Auditing;

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
                logger.LogError($"Invalid AuditMessage received: {message}");
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
            Created = DateTime.Now,
            HttpRequestType = httpRequestType.ToString(),
            ClaimId = id.ToString("D")
        };

        auditContext.Add(claimAudit);
        auditContext.SaveChanges();
    }
}