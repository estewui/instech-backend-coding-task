using FluentValidation.TestHelper;
using Xunit;

using Application.Covers.CreateCover;
using Domain.Entities;

namespace Claims.Tests.Application
{
    public class CreateCoverValidatorTests
    {
        private readonly CreateCoverValidator _validator;

        public CreateCoverValidatorTests()
        {
            _validator = new CreateCoverValidator();
        }

        [Fact]
        public void Validate_ShouldPass_WhenCoverIsValid()
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(31), CoverType.Yacht, 5000m)
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
            var cover = new Cover(DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(1), CoverType.Yacht, 5000m)
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
            var sameDate = DateTime.UtcNow.AddDays(10);
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
            var cover = new Cover(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30), CoverType.Yacht, 5000m)
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
            // Arrange - DateTime.UtcNow in the validator will be slightly after this
            var cover = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert - This might pass or fail depending on timing, but typically should fail
            // because the validator's DateTime.UtcNow is evaluated after the cover's StartDate
            result.ShouldHaveValidationErrorFor(c => c.StartDate);
        }

        [Fact]
        public void Validate_ShouldFail_WhenInsurancePeriodExceeds365Days()
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(367), CoverType.Yacht, 5000m)
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
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(365); // Exactly 365 days
            var a = (endDate - startDate).TotalDays; // Should be 366 days including the start date
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
            var cover = new Cover(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(100), CoverType.Yacht, 5000m)
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
            var cover = new Cover(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), CoverType.Yacht, 5000m)
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
            var cover = new Cover(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(31), coverType, 5000m)
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
            var cover = new Cover(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-20), CoverType.Yacht, 5000m)
            {
                Id = "cover-1"
            };

            // Act
            var result = _validator.TestValidate(cover);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.StartDate);
            Assert.True(result.Errors.Count >= 2); // Should have at least 2 errors
        }

        [Fact]
        public void Validate_ShouldFail_WhenBothDateValidationsAndPeriodValidationFail()
        {
            // Arrange - dates in past AND period exceeds 1 year
            var cover = new Cover(DateTime.UtcNow.AddDays(-400), DateTime.UtcNow.AddDays(-1), CoverType.Yacht, 5000m)
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
            var cover = new Cover(DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(100), CoverType.PassengerShip, 10000m)
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
        [InlineData(30, 395)] // Exactly 365 days
        public void Validate_ShouldPass_ForVariousValidDateRanges(int startDaysFromNow, int endDaysFromNow)
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow.AddDays(startDaysFromNow), DateTime.UtcNow.AddDays(endDaysFromNow), CoverType.Yacht, 5000m)
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
