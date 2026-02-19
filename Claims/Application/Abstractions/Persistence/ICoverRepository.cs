using Domain.Entities;

namespace Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines methods for managing insurance covers in a repository.
    /// </summary>
    public interface ICoverRepository
    {
        /// <summary>
        /// Creates a new cover.
        /// </summary>
        /// <param name="cover">The cover to create.</param>
        /// <returns>The created cover.</returns>
        Cover Create(Cover cover);
        /// <summary>
        /// Deletes a cover by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the cover to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteById(string id);
        /// <summary>
        /// Retrieves all covers asynchronously.
        /// </summary>
        /// <returns>A list of all covers.</returns>
        Task<List<Cover>> GetAll();
        /// <summary>
        /// Retrieves a cover by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the cover.</param>
        /// <returns>The cover with the specified identifier.</returns>
        Task<Cover> GetById(string id);
    }
}
