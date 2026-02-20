using AutoMapper;

using Application.Abstractions.Persistence;
using Application.Common.Auditing;
using SqlModels = Infrastructure.Persistence.Sql.Models;

namespace Infrastructure.Persistence.Sql.Repositories
{
    /// <summary>
    /// Repository for auditing claim and cover actions in SQL Server.
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly AuditContext _db;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRepository"/> class.
        /// </summary>
        /// <param name="db">The audit database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public AuditRepository(AuditContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Audits a claim action.
        /// </summary>
        /// <param name="auditEvent">The audit event containing claim audit information.</param>
        public void AuditClaim(AuditEvent auditEvent)
        {
            var claimAudit = new SqlModels.ClaimAudit
            {
                ClaimId = auditEvent.EntityId,
                Created = auditEvent.Timestamp,
                HttpRequestType = auditEvent.HttpRequestType
            };

            _db.Add(claimAudit);
            _db.SaveChanges();
        }

        /// <summary>
        /// Audits a cover action.
        /// </summary>
        /// <param name="auditEvent">The audit event containing cover audit information.</param>
        public void AuditCover(AuditEvent auditEvent)
        {
            var coverAudit = new SqlModels.CoverAudit
            {
                CoverId = auditEvent.EntityId,
                Created = auditEvent.Timestamp,
                HttpRequestType = auditEvent.HttpRequestType
            };

            _db.Add(coverAudit);
            _db.SaveChanges();
        }
    }
}
