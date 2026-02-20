using Xunit;

using Domain.Entities;
using Premium = Domain.Services.PremiumCalculator;

namespace Claims.Tests.Domain
{
    public class ComputePremiumTests
    {
        [Theory]
        [InlineData(CoverType.Yacht, "2025-02-21 12:00:00", "2025-02-23 14:00:00", 4125)] // 1250 * 1.1 * 3 days (ceiling from 2.08 days)
        [InlineData(CoverType.Yacht, "2025-02-21 15:00:00", "2025-02-23 13:30:00", 2750)] // 1250 * 1.1 * 2 days
        public void ComputePremium_ShouldCalculateForCorrectAmountOfDays(CoverType coverType, string startAt, string endAt, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.Parse(startAt);
            var endDate = DateTime.Parse(endAt);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }

        // 1250 * 1.1 = 1375 (price for yacht for days 1-30)
        // 1375 * 0.95 = 1306.25 (price for yacht for days 31-180)
        // 1375 * 0.92 = 1265 (price for yacht for days 181+)
        [Theory]
        [InlineData(CoverType.Yacht, 29, 39875)] // 1375 * 29 days
        [InlineData(CoverType.Yacht, 30, 41250)] // 1375 * 30 days
        [InlineData(CoverType.Yacht, 31, 42556.25)] // 1375 * 30 days + 1306.25 * 1 day
        [InlineData(CoverType.Yacht, 179, 235881.25)] // 1375 * 30 days + 1306.25 * 149 days
        [InlineData(CoverType.Yacht, 180, 237187.5)] // 1375 * 30 days + 1306.25 * 150 days
        [InlineData(CoverType.Yacht, 181, 238452.5)] // 1375 * 30 days + 1306.25 * 150 days + 1265 * 1 day
        [InlineData(CoverType.Yacht, 300, 388987.5)] // 1375 * 30 days + 1306.25 * 150 days + 1265 * 120 day
        public void ComputePremium_ShouldCalculateCorrectly_ForYacht(CoverType coverType, int days, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }

        // 1250 * 1.2 = 1500 (price for passenger ship for days 1-30)
        // 1500 * 0.98 = 1470 (price for passenger ship for days 31-180)
        // 1500 * 0.97 = 1455 (price for passenger ship for days 181+)
        [Theory]
        [InlineData(CoverType.PassengerShip, 29, 43500)] // 1500 * 29 days
        [InlineData(CoverType.PassengerShip, 30, 45000)] // 1500 * 30 days
        [InlineData(CoverType.PassengerShip, 31, 46470)] // 1500 * 30 days + 1470 * 1 day
        [InlineData(CoverType.PassengerShip, 179, 264030)] // 1500 * 30 days + 1470 * 149 days
        [InlineData(CoverType.PassengerShip, 180, 265500)] // 1500 * 30 days + 1470 * 150 days
        [InlineData(CoverType.PassengerShip, 181, 266955)] // 1500 * 30 days + 1470 * 150 days + 1455 * 1 day
        [InlineData(CoverType.PassengerShip, 300, 440100)] // 1500 * 30 days + 1470 * 150 days + 1455 * 120 day
        public void ComputePremium_ShouldCalculateCorrectly_ForPassengerShip(CoverType coverType, int days, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }

        // 1250 * 1.5 = 1875 (price for tanker for days 1-30)
        // 1875 * 0.98 = 1837.5 (price for tanker for days 31-180)
        // 1875 * 0.97 = 1818.75 (price for tanker for days 181+)
        [Theory]
        [InlineData(CoverType.Tanker, 29, 54375)] // 1875 * 29 days
        [InlineData(CoverType.Tanker, 30, 56250)] // 1875 * 30 days
        [InlineData(CoverType.Tanker, 31, 58087.5)] // 1875 * 30 days + 1837.5 * 1 day
        [InlineData(CoverType.Tanker, 179, 330037.5)] // 1875 * 30 days + 1837.5 * 149 days
        [InlineData(CoverType.Tanker, 180, 331875)] // 1875 * 30 days + 1837.5 * 150 days
        [InlineData(CoverType.Tanker, 181, 333693.75)] // 1875 * 30 days + 1837.5 * 150 days + 1818.75 * 1 day
        [InlineData(CoverType.Tanker, 300, 550125)] // 1875 * 30 days + 1837.5 * 150 days + 1818.75 * 120 day
        public void ComputePremium_ShouldCalculateCorrectly_ForTanker(CoverType coverType, int days, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }

        // 1250 * 1.3 = 1625 (price for container ship for days 1-30)
        // 1625 * 0.98 = 1592.5 (price for container ship for days 31-180)
        // 1625 * 0.97 = 1576.25 (price for container ship for days 181+)
        [Theory]
        [InlineData(CoverType.ContainerShip, 29, 47125)] // 1625 * 29 days
        [InlineData(CoverType.ContainerShip, 30, 48750)] // 1625 * 30 days
        [InlineData(CoverType.ContainerShip, 31, 50342.5)] // 1625 * 30 days + 1592.5 * 1 day
        [InlineData(CoverType.ContainerShip, 179, 286032.5)] // 1625 * 30 days + 1592.5 * 149 days
        [InlineData(CoverType.ContainerShip, 180, 287625)] // 1625 * 30 days + 1592.5 * 150 days
        [InlineData(CoverType.ContainerShip, 181, 289201.25)] // 1625 * 30 days + 1592.5 * 150 days + 1576.25 * 1 day
        [InlineData(CoverType.ContainerShip, 300, 476775)] // 1625 * 30 days + 1592.5 * 150 days + 1576.25 * 120 day
        public void ComputePremium_ShouldCalculateCorrectly_ForContainerShip(CoverType coverType, int days, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }

        // 1250 * 1.3 = 1625 (price for bulk carrier for days 1-30)
        // 1625 * 0.98 = 1592.5 (price for bulk carrier for days 31-180)
        // 1625 * 0.97 = 1576.25 (price for bulk carrier for days 181+)
        [Theory]
        [InlineData(CoverType.BulkCarrier, 29, 47125)] // 1625 * 29 days
        [InlineData(CoverType.BulkCarrier, 30, 48750)] // 1625 * 30 days
        [InlineData(CoverType.BulkCarrier, 31, 50342.5)] // 1625 * 30 days + 1592.5 * 1 day
        [InlineData(CoverType.BulkCarrier, 179, 286032.5)] // 1625 * 30 days + 1592.5 * 149 days
        [InlineData(CoverType.BulkCarrier, 180, 287625)] // 1625 * 30 days + 1592.5 * 150 days
        [InlineData(CoverType.BulkCarrier, 181, 289201.25)] // 1625 * 30 days + 1592.5 * 150 days + 1576.25 * 1 day
        [InlineData(CoverType.BulkCarrier, 300, 476775)] // 1625 * 30 days + 1592.5 * 150 days + 1576.25 * 120 day
        public void ComputePremium_ShouldCalculateCorrectly_ForBulkCarrier(CoverType coverType, int days, decimal expectedPremium)
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            // Act
            var result = Premium.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(expectedPremium, result);
        }
    }
}
