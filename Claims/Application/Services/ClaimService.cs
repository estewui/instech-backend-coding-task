using Application.Abstractions.Persistence;
using Domain.Entities;
using FluentValidation;

namespace Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IValidator<Claim> _validator;

        public ClaimService(IClaimRepository claimRepository, IValidator<Claim> validator)
        {
            _claimRepository = claimRepository;
            _validator = validator;
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
            await _validator.ValidateAndThrowAsync(claim);

            return await _claimRepository.Create(claim);
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            return await _claimRepository.GetAll();
        }
    }
}