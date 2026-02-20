using Domain.Entities;

namespace Application.Services
{
    /// <summary>
    /// Defines methods for managing insurance covers.
    /// </summary>
    public interface ICoverService
    {
        /// <summary>
        /// Computes the premium for a cover based on dates and cover type.
        /// </summary>
        /// <param name="startDate">The start date of the cover.</param>
        /// <param name="endDate">The end date of the cover.</param>
        /// <param name="coverType">The type of the cover.</param>
        /// <returns>The computed premium.</returns>
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
        /// <summary>
        /// Creates a new cover.
        /// </summary>
        /// <param name="cover">The cover to create.</param>
        /// <returns>The created cover.</returns>
        Task<Cover> Create(Cover cover);
        /// <summary>
        /// Deletes a cover by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the cover to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteById(string id);
        /// <summary>
        /// Retrieves all covers asynchronously.
        /// </summary>
        /// <returns>A collection of all covers.</returns>
        Task<IEnumerable<Cover>> GetAll();
        /// <summary>
        /// Retrieves a cover by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the cover.</param>
        /// <returns>The cover with the specified identifier, or null if not found.</returns>
        Task<Cover?> GetById(string id);
    }
}
