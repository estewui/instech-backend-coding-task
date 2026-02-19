using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Application.Services
{
    public class ClaimService: IClaimService
    {
        private readonly IClaimRepository _claimRepository;
    
        public ClaimService(IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }

        public async Task<Claim> GetClaimByIdAsync(string id)
        {
            return await _claimRepository.GetById(id);
        }

        public void DeleteClaimById(string id)
        {
            _claimRepository.DeleteById(id);
        }

        public async Task<Claim> CreateClaimAsync(Claim claim)
        {
            return await _claimRepository.Create(claim);
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            return await _claimRepository.GetAll();
        }
    }
}
