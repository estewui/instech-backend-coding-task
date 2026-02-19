using Application.Abstractions.Persistence;
using Application.Common.Auditing;

namespace Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _audit;
        public AuditService(IAuditRepository audit)
        {
            _audit = audit;
        }
        public void AuditClaim(AuditEvent auditEvent)
        {
            _audit.AuditClaim(auditEvent);
        }

        public void AuditCover(AuditEvent auditEvent)
        {
            _audit.AuditCover(auditEvent);
        }
    }
}
