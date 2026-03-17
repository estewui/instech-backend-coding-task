using FluentValidation.TestHelper;
using Moq;
using Xunit;

using Application.Abstractions.Persistence;
using Application.Claims.CreateClaim;
using Domain.Entities;

namespace Claims.Tests.Application
{
    public class CreateClaimValidatorTests
    {
        private static readonly DateTime FixedNow = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        private readonly Mock<ICoverRepository> _mockCoverRepository;
        private readonly CreateClaimValidator _validator;

        public CreateClaimValidatorTests()
        {
            _mockCoverRepository = new Mock<ICoverRepository>();
            _validator = new CreateClaimValidator(_mockCoverRepository.Object);
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenClaimIsValid()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenCoverDoesNotExist()
        {
            // Arrange
            var claim = new Claim("non-existent-cover", FixedNow, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((Cover?)null);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c)
                .WithErrorMessage("Created date must be within the period of the related Cover.");
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenCreatedDateIsBeforeCoverStartDate()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(5), FixedNow.AddDays(35), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c)
                .WithErrorMessage("Created date must be within the period of the related Cover.");
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenCreatedDateIsAfterCoverEndDate()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-30), FixedNow.AddDays(-5), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c)
                .WithErrorMessage("Created date must be within the period of the related Cover.");
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenCreatedDateIsOnCoverStartDate()
        {
            // Arrange
            var coverId = "cover-1";
            var startDate = FixedNow;
            var cover = new Cover(startDate, startDate.AddDays(30), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, startDate, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenCreatedDateIsOnCoverEndDate()
        {
            // Arrange
            var coverId = "cover-1";
            var endDate = FixedNow.AddDays(30);
            var cover = new Cover(FixedNow, endDate, CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, endDate, "Test Claim", ClaimType.Collision, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenDamageCostIsNegative()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, -1000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.DamageCost)
                .WithErrorMessage("DamageCost must be greater than 0.");
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenDamageCostExceeds100000()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 100001m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.DamageCost)
                .WithErrorMessage("DamageCost cannot exceed 100.000.");
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenDamageCostIsZero()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 0m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.DamageCost)
                .WithErrorMessage("DamageCost must be greater than 0.");
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenDamageCostIsExactly100000()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", ClaimType.Collision, 100000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(ClaimType.Collision)]
        [InlineData(ClaimType.Grounding)]
        [InlineData(ClaimType.BadWeather)]
        [InlineData(ClaimType.Fire)]
        public async Task Validate_ShouldPass_ForAllClaimTypes(ClaimType claimType)
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(FixedNow.AddDays(-10), FixedNow.AddDays(20), CoverType.Yacht, 5000m)
            {
                Id = coverId
            };
            var claim = new Claim(coverId, FixedNow, "Test Claim", claimType, 50000m)
            {
                Id = "claim-1"
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
