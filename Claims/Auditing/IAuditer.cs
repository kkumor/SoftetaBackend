namespace Claims.Auditing;

public interface IAuditer
{
    Task Audit(AuditTypes type, Guid id, AuditHttpRequestType requestType);
}