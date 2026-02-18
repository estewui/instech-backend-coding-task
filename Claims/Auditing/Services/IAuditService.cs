namespace Auditing.Services
{
    public interface IAuditService
    {
        void AuditClaim(string id, string httpRequestType);
        void AuditCover(string id, string httpRequestType);
    }
}