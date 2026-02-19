using Domain.Entities;

namespace Application.Services
{
    public interface IClaimService
    {
        Task<Claim> CreateClaimAsync(Claim claim);
        void DeleteClaimById(string id);
        Task<Claim> GetClaimByIdAsync(string id);
        Task<List<Claim>> GetClaimsAsync();
    }
}
