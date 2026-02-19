using Domain.Entities;

namespace Application.Abstractions.Persistence
{
    public interface ICoverRepository
    {
        Cover Create(Cover cover);
        Task DeleteById(string id);
        Task<List<Cover>> GetAll();
        Task<Cover> GetById(string id);
    }
}
