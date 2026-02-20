using Application.Abstractions.Persistence;
using Domain.Entities;
using FluentValidation;
using System.Security.Claims;
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

        public const decimal BASE_DAY_RATE = 1250m;
        public const decimal DEFAULT_MULTIPLIER = 1.3m;
        public const decimal DEFAULT_AFTER_30_BEFORE_180_DAYS_MULTIPLIER = 0.98m;
        public const decimal DEFAULT_AFTER_150_DAYS_MULTIPLIER = 0.99m;

        public class CoverMultiplier
        {
            public decimal BaseDayRate { get; set; } = BASE_DAY_RATE;
            public decimal BaseMultiplier { get; set; } = DEFAULT_MULTIPLIER;
            public decimal After30Before180DaysMultiplier { get; set; } = DEFAULT_AFTER_30_BEFORE_180_DAYS_MULTIPLIER;
            public decimal After150DaysMultiplier { get; set; } = DEFAULT_AFTER_150_DAYS_MULTIPLIER;
        }

        public static CoverMultiplier DEFAULT_COVER_MULTIPLIER = new CoverMultiplier();

        public static Dictionary<CoverType, CoverMultiplier> MULTIPLIERS = new Dictionary<CoverType, CoverMultiplier>
            {
                { CoverType.Yacht, new CoverMultiplier { BaseMultiplier = 1.1m, After30Before180DaysMultiplier = 0.95m, After150DaysMultiplier = 0.97m } },
                { CoverType.PassengerShip, new CoverMultiplier { BaseMultiplier = 1.2m } },
                { CoverType.ContainerShip, new CoverMultiplier() },
                { CoverType.BulkCarrier, new CoverMultiplier() },
                { CoverType.Tanker, new CoverMultiplier { BaseMultiplier = 1.5m } }
            };


        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var coverMultiplier = MULTIPLIERS.TryGetValue(coverType, out CoverMultiplier multiplier) ? multiplier : DEFAULT_COVER_MULTIPLIER;
            var totalDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var totalDays1stPeriod = Math.Max(Math.Min(totalDays, 30), 0); // total days between day 0 and day 30, sets 0 if value is negative
            var totalDays2ndPeriod = Math.Max(Math.Min(totalDays - 30, 150), 0); // total days between day 30 and day 180 (150 days after day 30), sets 0 if value is negative
            var totalDays3rdPeriod = Math.Max(totalDays - 180, 0); // remaining total days after day 180, sets 0 if value is negative

            var totalPremium = totalDays1stPeriod * coverMultiplier.BaseDayRate * coverMultiplier.BaseMultiplier
                             + totalDays2ndPeriod * coverMultiplier.BaseDayRate * coverMultiplier.After30Before180DaysMultiplier
                             + totalDays3rdPeriod * coverMultiplier.BaseDayRate * coverMultiplier.After150DaysMultiplier;

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
