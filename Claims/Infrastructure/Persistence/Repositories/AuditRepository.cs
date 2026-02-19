using Application.Abstractions.Persistence;
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

        public void AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            _db.Add(claimAudit);
            _db.SaveChanges();
        }

        public void AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            _db.Add(coverAudit);
            _db.SaveChanges();
        }
    }
}
