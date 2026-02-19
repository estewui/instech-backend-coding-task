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
        /// <param name="id">The claim identifier.</param>
        /// <param name="httpRequestType">The HTTP request type (e.g., POST, DELETE).</param>
        void AuditClaim(string id, string httpRequestType);
        /// <summary>
        /// Audits a cover action.
        /// </summary>
        /// <param name="id">The cover identifier.</param>
        /// <param name="httpRequestType">The HTTP request type (e.g., POST, DELETE).</param>
        void AuditCover(string id, string httpRequestType);
    }
}
