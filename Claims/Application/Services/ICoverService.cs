using Domain.Entities;

namespace Application.Services
{
    public interface ICoverService
    {
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
        Cover Create(Cover cover);
        Task DeleteById(string id);
        Task<IEnumerable<Cover>> GetAll();
        Task<Cover> GetById(string id);
    }
}
