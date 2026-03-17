using FluentValidation.TestHelper;
using Xunit;

using Application.Covers.CreateCover;
using Domain.Entities;

namespace Claims.Tests.Application
{
    public class CreateCoverValidatorTests
    {
        private static readonly DateTimeOffset FixedNow = new(2025, 6, 15, 12, 0, 0, TimeSpan.Zero);
        private readonly CreateCoverValidator _validator;

        public CreateCoverValidatorTests()
        {
            var fakeTimeProvider = new FakeTimeProvider(FixedNow);
            _validator = new CreateCoverValidator(fakeTimeProvider);
        }

        [Fact]
        public void Validate_ShouldPass_WhenCoverIsValid()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(1), FixedNow.UtcDateTime.AddDays(31), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldFail_WhenStartDateIsAfterEndDate()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(30), FixedNow.UtcDateTime.AddDays(1), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate)
                .WithErrorMessage("Start date has to be before End date.");
        }

        [Fact]
        public void Validate_ShouldFail_WhenStartDateEqualsEndDate()
        {
            // Arrange
            var sameDate = FixedNow.UtcDateTime.AddDays(10);
            var cover = new Cover(sameDate, sameDate, CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate)
                .WithErrorMessage("Start date has to be before End date.");
        }

        [Fact]
        public void Validate_ShouldFail_WhenStartDateIsInThePast()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(-1), FixedNow.UtcDateTime.AddDays(30), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate)
                .WithErrorMessage("Start date cannot be in the past.");
        }

        [Fact]
        public void Validate_ShouldFail_WhenStartDateIsNow()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime, FixedNow.UtcDateTime.AddDays(30), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert — deterministic now: StartDate == FixedNow is not > FixedNow, so this always fails
            result.ShouldHaveValidationErrorFor(c => c.StartDate);
        }

        [Fact]
        public void Validate_ShouldFail_WhenInsurancePeriodExceeds365Days()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(1), FixedNow.UtcDateTime.AddDays(367), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c)
                .WithErrorMessage("Total insurance period cannot exceed 1 year");
        }

        [Fact]
        public void Validate_ShouldPass_WhenInsurancePeriodIsExactly365Days()
        {
            // Arrange
            var startDate = FixedNow.UtcDateTime.AddDays(1);
            var endDate = startDate.AddDays(365);
            var cover = new Cover(startDate, endDate, CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInsurancePeriodIsLessThan365Days()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(1), FixedNow.UtcDateTime.AddDays(100), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInsurancePeriodIs1Day()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(1), FixedNow.UtcDateTime.AddDays(2), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(CoverType.Yacht)]
        [InlineData(CoverType.PassengerShip)]
        [InlineData(CoverType.ContainerShip)]
        [InlineData(CoverType.BulkCarrier)]
        [InlineData(CoverType.Tanker)]
        public void Validate_ShouldPass_ForAllCoverTypes(CoverType coverType)
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(1), FixedNow.UtcDateTime.AddDays(31), coverType, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenMultipleRulesFail()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(-10), FixedNow.UtcDateTime.AddDays(-20), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate);
            Assert.True(result.Errors.Count >= 2);
        }

        [Fact]
        public void Validate_ShouldFail_WhenBothDateValidationsAndPeriodValidationFail()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(-400), FixedNow.UtcDateTime.AddDays(-1), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate);
            result.ShouldHaveValidationErrorFor(c => c);
        }

        [Fact]
        public void Validate_ShouldPass_WhenCoverStartsInFutureAndEndsWithin365Days()
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(10), FixedNow.UtcDateTime.AddDays(100), CoverType.PassengerShip, 10000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 35)]
        [InlineData(10, 100)]
        [InlineData(30, 395)]
        public void Validate_ShouldPass_ForVariousValidDateRanges(int startDaysFromNow, int endDaysFromNow)
        {
            // Arrange
            var cover = new Cover(FixedNow.UtcDateTime.AddDays(startDaysFromNow), FixedNow.UtcDateTime.AddDays(endDaysFromNow), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
