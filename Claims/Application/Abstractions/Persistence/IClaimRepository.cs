using Domain.Entities;

namespace Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines methods for managing insurance claims in a repository.
    /// </summary>
    public interface IClaimRepository
    {
        /// <summary>
        /// Creates a new claim asynchronously.
        /// </summary>
        /// <param name="claim">The claim to create.</param>
        /// <returns>The created claim.</returns>
        Task<Claim> Create(Claim claim, CancellationToken cancellationToken);
        /// <summary>
        /// Deletes a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteById(string id, CancellationToken cancellationToken);
        /// <summary>
        /// Retrieves all claims asynchronously.
        /// </summary>
        /// <returns>A list of all claims.</returns>
        Task<List<Claim>> GetAll(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a claim by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the claim.</param>
        /// <returns>The claim with the specified identifier, or null if not found.</returns>
        Task<Claim?> GetById(string id, CancellationToken cancellationToken);
    }
}
