using Domain.Entities;

namespace Domain.Services
{
    public class CoverMultiplier
    {
        private static readonly decimal BASE_DAY_RATE = 1250m;
        private static readonly decimal DEFAULT_MULTIPLIER = 1.3m;
        private static readonly decimal DEFAULT_AFTER_30_BEFORE_180_DAYS_DISCOUNT = 0.02m;
        private static readonly decimal DEFAULT_AFTER_150_DAYS_ADDITIONAL_DISCOUNT = 0.01m;

        public decimal BaseDayRate { get; set; } = BASE_DAY_RATE;
        public decimal BaseMultiplier { get; set; } = DEFAULT_MULTIPLIER;
        public decimal After30Before180DaysDiscount { get; set; } = DEFAULT_AFTER_30_BEFORE_180_DAYS_DISCOUNT;
        public decimal After150DaysDiscount { get; set; } = DEFAULT_AFTER_150_DAYS_ADDITIONAL_DISCOUNT;
    }
    
    public static class PremiumCalculator
    {
        private static readonly CoverMultiplier DEFAULT_COVER_MULTIPLIER = new CoverMultiplier();

        private static readonly Dictionary<CoverType, CoverMultiplier> MULTIPLIERS = new Dictionary<CoverType, CoverMultiplier>
            {
                { CoverType.Yacht, new CoverMultiplier { BaseMultiplier = 1.1m, After30Before180DaysDiscount = 0.05m, After150DaysDiscount = 0.03m } },
                { CoverType.PassengerShip, new CoverMultiplier { BaseMultiplier = 1.2m } },
                { CoverType.ContainerShip, new CoverMultiplier() },
                { CoverType.BulkCarrier, new CoverMultiplier() },
                { CoverType.Tanker, new CoverMultiplier { BaseMultiplier = 1.5m } }
            };

        /* Instructions on github are a little bit unclear to me. Days 31-180 are discounted by {X}%, and days 181+ are discounted by an ADDITIONAl {Y}%
         My concern is, it's not explicitally explained if the second discount (Y %) should be applied to the initial base rate, or to the 'middle' base rate from days 31-180
         For example, if initial base day rate would be 100$, X=2%, Y=1%, base multiplier = 1 then:
            scenario A (discount applied to the initial base rate) - price for day 31 would be 100 * (1 - 0.02) = 98$, and price for day 181 would be 100 * (1 - (0.02 + 0.01)) = 97$
            scenario B (discount applied to the 'middle' base rate) - price for day 31 would be 100 * (1 - 0.02) = 98$, and price for day 181 would be 98 * (1 - 0.01) = 97.02$
         In my solution, I decided to choose scenario A, as it was already implemented in the code before and seems more straightforward.
         In case I would like to implement scenario B, I would only adjust one line: var after150DaysRate = after30daysBefore180DaysRate * (1.00m - (coverMultiplier.After150DaysDiscount));
        */
        public static decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var coverMultiplier = MULTIPLIERS.TryGetValue(coverType, out CoverMultiplier? multiplier) ? multiplier : DEFAULT_COVER_MULTIPLIER;
            var totalDays = (int)Math.Ceiling((endDate - startDate).TotalDays);

            var totalDays1stPeriod = Math.Max(Math.Min(totalDays, 30), 0); // total days between day 0 and day 30, sets 0 if value is negative
            var totalDays2ndPeriod = Math.Max(Math.Min(totalDays - 30, 150), 0); // total days between day 30 and day 180 (150 days after day 30), sets 0 if value is negative
            var totalDays3rdPeriod = Math.Max(totalDays - 180, 0); // remaining total days after day 180, sets 0 if value is negative

            var initialBaseRate = coverMultiplier.BaseDayRate * coverMultiplier.BaseMultiplier;
            var after30daysBefore180DaysRate = initialBaseRate * (1.00m - coverMultiplier.After30Before180DaysDiscount);
            var after150DaysRate = initialBaseRate * (1.00m - (coverMultiplier.After30Before180DaysDiscount + coverMultiplier.After150DaysDiscount));

            var totalPremium = totalDays1stPeriod * initialBaseRate
                             + totalDays2ndPeriod * after30daysBefore180DaysRate
                             + totalDays3rdPeriod * after150DaysRate;

            return totalPremium;
        }
    }
}
