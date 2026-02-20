using FluentValidation;
using Moq;
using Xunit;

using Application.Abstractions.Persistence;
using Application.Services;
using Domain.Entities;

namespace Claims.Tests.Application
{
    /// <summary>
    /// Unit tests for ClaimService.
    /// </summary>
    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> _mockClaimRepository;
        private readonly Mock<IValidator<Claim>> _mockValidator;
        private readonly ClaimService _claimService;

        public ClaimServiceTests()
        {
            _mockClaimRepository = new Mock<IClaimRepository>();
            _mockValidator = new Mock<IValidator<Claim>>();
            _claimService = new ClaimService(_mockClaimRepository.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetClaimByIdAsync_ShouldReturnClaim_WhenClaimExists()
        {
            // Arrange
            var claimId = "claim-1";
            var expectedClaim = new Claim
            {
                Id = claimId,
                CoverId = "cover-1",
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 5000m,
                Created = DateTime.UtcNow
            };
            _mockClaimRepository.Setup(r => r.GetById(claimId)).ReturnsAsync(expectedClaim);

            // Act
            var result = await _claimService.GetClaimByIdAsync(claimId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(claimId, result.Id);
            Assert.Equal("Test Claim", result.Name);
            _mockClaimRepository.Verify(r => r.GetById(claimId), Times.Once);
        }

        [Fact]
        public async Task GetClaimsAsync_ShouldReturnAllClaims()
        {
            // Arrange
            var expectedClaims = new List<Claim>
            {
                new Claim { Id = "1", Name = "Claim 1", CoverId = "cover-1", Type = ClaimType.Fire, DamageCost = 1000m, Created = DateTime.UtcNow },
                new Claim { Id = "2", Name = "Claim 2", CoverId = "cover-2", Type = ClaimType.Grounding, DamageCost = 2000m, Created = DateTime.UtcNow }
            };
            _mockClaimRepository.Setup(r => r.GetAll()).ReturnsAsync(expectedClaims);

            // Act
            var result = await _claimService.GetClaimsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockClaimRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public async Task CreateClaimAsync_ShouldCreateClaim()
        {
            // Arrange
            var newClaim = new Claim
            {
                CoverId = "cover-1",
                Name = "New Claim",
                Type = ClaimType.BadWeather,
                DamageCost = 3000m,
                Created = DateTime.UtcNow
            };
            _mockClaimRepository.Setup(r => r.Create(It.IsAny<Claim>())).ReturnsAsync(newClaim);

            // Act
            var result = await _claimService.CreateClaimAsync(newClaim);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Claim", result.Name);
            _mockClaimRepository.Verify(r => r.Create(It.IsAny<Claim>()), Times.Once);
        }

        [Fact]
        public void DeleteClaimById_ShouldCallRepository()
        {
            // Arrange
            var claimId = "claim-1";
            _mockClaimRepository.Setup(r => r.DeleteById(claimId)).Returns(Task.CompletedTask);

            // Act
            _claimService.DeleteClaimById(claimId);

            // Assert
            _mockClaimRepository.Verify(r => r.DeleteById(claimId), Times.Once);
        }
    }
}
