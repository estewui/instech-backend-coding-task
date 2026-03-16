using FluentValidation;

using Application.Abstractions.Persistence;
using Domain.Entities;

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

        public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _claimRepository.GetById(id, cancellationToken);
        }

        public async Task DeleteClaimById(string id, CancellationToken cancellationToken)
        {
            await _claimRepository.DeleteById(id, cancellationToken);
        }

        public async Task<Claim> CreateClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(claim);

            return await _claimRepository.Create(claim, cancellationToken);
        }

        public async Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
        {
            return await _claimRepository.GetAll(cancellationToken);
        }
    }
}