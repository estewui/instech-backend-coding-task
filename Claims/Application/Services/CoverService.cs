using FluentValidation;

using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Services;

namespace Application.Services
{
    public class CoverService: ICoverService
    {
        private readonly ICoverRepository _coverRepository;
        private readonly IValidator<Cover> _validator;

        public CoverService(ICoverRepository coverRepository, IValidator<Cover> validator)
        {
            _coverRepository = coverRepository;
            _validator = validator;
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return PremiumCalculator.ComputePremium(startDate, endDate, coverType);
        }

        public Task<Cover?> GetById(string id, CancellationToken cancellationToken)
        {
            return _coverRepository.GetById(id, cancellationToken);
        }

        public async Task<IEnumerable<Cover>> GetAll(CancellationToken cancellationToken)
        {
            return await _coverRepository.GetAll(cancellationToken);
        }

        public async Task<Cover> Create(Cover cover, CancellationToken cancellationToken)
        {
            _validator.ValidateAndThrow(cover);

            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            return await _coverRepository.Create(cover, cancellationToken);
        }

        public Task DeleteById(string id, CancellationToken cancellationToken)
        {
            return _coverRepository.DeleteById(id, cancellationToken);
        }
    }
}
