namespace Auditing.Infrastructure.Interfaces
{
    public interface IDalAuditService
    {
        public void AuditClaim(string id, string httpRequestType);
        public void AuditCover(string id, string httpRequestType);
    }
}
