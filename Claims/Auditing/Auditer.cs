using Claims.Application.Services;
using Claims.Model;
using MassTransit;

namespace Claims.Auditing;

public class Auditer(AuditContext auditContext, IPublishEndpoint massTransitPublish) : IAuditerService
{
    public Task Audit(AuditTypes type, Guid id, AuditHttpRequestType requestType) =>
        massTransitPublish.Publish(new AuditMessage(type, id, requestType));

    public void AuditCover(Guid id, string httpRequestType)
    {
        var coverAudit = new CoverAudit
        {
            Created = DateTime.Now,
            HttpRequestType = httpRequestType,
            CoverId = id.ToString("D")
        };

        auditContext.Add(coverAudit);
        auditContext.SaveChanges();
    }
}