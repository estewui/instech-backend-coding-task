namespace Auditing.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditService _dalAuditService;
        public AuditService(IAuditService dalAuditService)
        {
            _dalAuditService = dalAuditService;
        }
        public void AuditClaim(string id, string httpRequestType)
        {
            _dalAuditService.AuditClaim(id, httpRequestType);
        }

        public void AuditCover(string id, string httpRequestType)
        {
            _dalAuditService.AuditCover(id, httpRequestType);
        }
    }
}
