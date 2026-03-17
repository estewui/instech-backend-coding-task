using Application.Common.Auditing;

namespace Application.Services
{
    public interface IAuditService
    {
        Task AuditClaim(AuditEvent auditEvent, CancellationToken ct);
        Task AuditCover(AuditEvent auditEvent, CancellationToken ct);
    }
}