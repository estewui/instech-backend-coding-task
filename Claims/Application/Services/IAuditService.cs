using Application.Common.Auditing;

namespace Application.Services
{
    public interface IAuditService
    {
        void AuditClaim(AuditEvent auditEvent);
        void AuditCover(AuditEvent auditEvent);
    }
}