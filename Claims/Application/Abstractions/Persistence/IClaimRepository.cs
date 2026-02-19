using Domain.Entities;

namespace Application.Abstractions.Persistence
{
    public interface IClaimRepository
    {
        Task<Claim> Create(Claim claim);
        Task DeleteById(string id);
        Task<List<Claim>> GetAll();
        Task<Claim> GetById(string id);
    }
}
