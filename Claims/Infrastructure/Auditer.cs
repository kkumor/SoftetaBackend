using Claims.Application.Auditing;
using Claims.Application.Services;
using Claims.Model;
using MassTransit;

namespace Claims.Infrastructure;

public class Auditer(AuditContext auditContext, IPublishEndpoint massTransitPublish) : IAuditerService
{
    public Task Audit(AuditTypes type, Guid id, AuditHttpRequestType requestType) =>
        massTransitPublish.Publish(new AuditMessage(type, id, requestType));
}