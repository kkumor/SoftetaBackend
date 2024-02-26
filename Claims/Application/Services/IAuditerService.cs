using Claims.Application.Auditing;

namespace Claims.Application.Services;

public interface IAuditerService
{
    Task Audit(AuditTypes type, Guid id, AuditHttpRequestType requestType);
}