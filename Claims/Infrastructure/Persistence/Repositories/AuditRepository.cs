using Application.Abstractions.Persistence;
using Application.Common.Auditing;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditContext _db;

        public AuditRepository(AuditContext db)
        {
            _db = db;
        }

        public void AuditClaim(AuditEvent auditEvent)
        {
            var claimAudit = new ClaimAudit()
            {
                ClaimId = auditEvent.EntityId,
                Created = auditEvent.Timestamp,
                HttpRequestType = auditEvent.HttpRequestType
            };

            _db.Add(claimAudit);
            _db.SaveChanges();
        }

        public void AuditCover(AuditEvent auditEvent)
        {
            var coverAudit = new CoverAudit()
            {
                CoverId = auditEvent.EntityId,
                Created = auditEvent.Timestamp,
                HttpRequestType = auditEvent.HttpRequestType,
            };

            _db.Add(coverAudit);
            _db.SaveChanges();
        }
    }
}
