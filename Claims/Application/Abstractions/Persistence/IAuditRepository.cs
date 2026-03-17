using Application.Common.Auditing;

namespace Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines methods for auditing claims and covers.
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Audits a claim action.
        /// </summary>
        /// <param name="auditEvent">The audit event.</param>
        Task AuditClaim(AuditEvent auditEven, CancellationToken ct);
        /// <summary>
        /// Audits a cover action.
        /// </summary>
        /// <param name="auditEvent">The audit event.</param>
        Task AuditCover(AuditEvent auditEvent, CancellationToken ct);
    }
}
