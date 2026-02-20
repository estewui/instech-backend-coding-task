using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Services;
using FluentValidation;
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

        public Task<Cover> GetById(string id)
        {
            return _coverRepository.GetById(id);
        }

        public async Task<IEnumerable<Cover>> GetAll()
        {
            return await _coverRepository.GetAll();
        }

        public async Task<Cover> Create(Cover cover)
        {
            _validator.ValidateAndThrow(cover);

            return await _coverRepository.Create(cover);
        }

        public Task DeleteById(string id)
        {
            return _coverRepository.DeleteById(id);
        }
    }
}
