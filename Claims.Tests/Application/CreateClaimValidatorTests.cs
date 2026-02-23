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
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = DateTime.UtcNow
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
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = "non-existent-cover",
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = DateTime.UtcNow
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
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(35),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = DateTime.UtcNow // Before cover start date
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
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(-5),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = DateTime.UtcNow // After cover end date
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
            var startDate = DateTime.UtcNow.Date;
            var cover = new Cover
            {
                Id = coverId,
                StartDate = startDate,
                EndDate = startDate.AddDays(30),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = startDate
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
            var endDate = DateTime.UtcNow.Date.AddDays(30);
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.Date,
                EndDate = endDate,
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 50000m,
                Created = endDate
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
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = -1000m,
                Created = DateTime.UtcNow
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.DamageCost)
                .WithErrorMessage("DamageCost cannot exceed 100.000.");
        }

        [Fact]
        public async Task Validate_ShouldFail_WhenDamageCostExceeds100000()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 100001m,
                Created = DateTime.UtcNow
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.DamageCost)
                .WithErrorMessage("DamageCost cannot exceed 100.000.");
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenDamageCostIsZero()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 0m,
                Created = DateTime.UtcNow
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Validate_ShouldPass_WhenDamageCostIsExactly100000()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 100000m,
                Created = DateTime.UtcNow
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
            var cover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            var claim = new Claim
            {
                Id = "claim-1",
                CoverId = coverId,
                Name = "Test Claim",
                Type = claimType,
                DamageCost = 50000m,
                Created = DateTime.UtcNow
            };

            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _validator.TestValidateAsync(claim, null, CancellationToken.None);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
