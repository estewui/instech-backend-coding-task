using Business.Models;

namespace DAL.Services
{
    public interface IDalClaimsService
    {
        Task<Claim> Create(Claim claim);
        Task DeleteById(string id);
        Task<List<Claim>> GetAll();
        Task<Claim> GetById(string id);
    }
}