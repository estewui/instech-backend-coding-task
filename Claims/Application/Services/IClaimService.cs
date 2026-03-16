using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// Defines methods for managing insurance claims.
    /// </summary>
    public interface IClaimService
    {
        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>The created claim.</returns>
        Task<Claim> CreateClaimAsync(Claim claim, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes a claim by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the claim to delete.</param>
        Task DeleteClaimById(string id, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The claim with the specified identifier, or null if not found.</returns>
        Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves all claims asynchronously.
        /// </summary>
        /// <returns>A list of all claims.</returns>
        Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken);
    }
}
