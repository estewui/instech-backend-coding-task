using Business.Models;

namespace DAL.Services
{
    public interface IDalCoversService
    {
        Cover Create(Cover cover);
        Task DeleteById(string id);
        Task<List<Cover>> GetAll();
        Task<Cover> GetById(string id);
    }
}