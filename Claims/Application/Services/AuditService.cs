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
        public async Task AuditClaim(AuditEvent auditEvent, CancellationToken ct)
        {
            await _audit.AuditClaim(auditEvent, ct);
        }

        public async Task AuditCover(AuditEvent auditEvent, CancellationToken ct)
        {
            await _audit.AuditCover(auditEvent, ct);
        }
    }
}
