using Application.Abstractions.Persistence;

namespace Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _audit;
        public AuditService(IAuditRepository audit)
        {
            _audit = audit;
        }
        public void AuditClaim(string id, string httpRequestType)
        {
            _audit.AuditClaim(id, httpRequestType);
        }

        public void AuditCover(string id, string httpRequestType)
        {
            _audit.AuditCover(id, httpRequestType);
        }
    }
}
