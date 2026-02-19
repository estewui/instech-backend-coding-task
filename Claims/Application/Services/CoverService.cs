using Application.Abstractions.Persistence;
using Domain.Entities;
namespace Application.Services
{
    public class CoverService: ICoverService
    {
        private readonly ICoverRepository _coverRepository;

        public CoverService(ICoverRepository coverRepository)
        {
            _coverRepository = coverRepository;
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = (endDate - startDate).TotalDays;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30) totalPremium += premiumPerDay;
                if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }

            return totalPremium;
        }

        public Task<Cover> GetById(string id)
        {
            return _coverRepository.GetById(id);
        }

        public async Task<IEnumerable<Cover>> GetAll()
        {
            return await _coverRepository.GetAll();
        }

        public Cover Create(Cover cover)
        {
            return _coverRepository.Create(cover);
        }

        public Task DeleteById(string id)
        {
            return _coverRepository.DeleteById(id);
        }
    }
}
