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
            var expectedClaim = new Claim("cover-1", DateTime.UtcNow, "Test Claim", ClaimType.Collision, 5000m)
            {
                Id = claimId
            };
            _mockClaimRepository.Setup(r => r.GetById(claimId, CancellationToken.None)).ReturnsAsync(expectedClaim);

            // Act
            var result = await _claimService.GetClaimByIdAsync(claimId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(claimId, result.Id);
            Assert.Equal("Test Claim", result.Name);
            _mockClaimRepository.Verify(r => r.GetById(claimId, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetClaimsAsync_ShouldReturnAllClaims()
        {
            // Arrange
            var expectedClaims = new List<Claim>
            {
                new Claim("cover-1", DateTime.UtcNow, "Claim 1", ClaimType.Fire, 1000m) { Id = "1" },
                new Claim("cover-2", DateTime.UtcNow, "Claim 2", ClaimType.Grounding, 2000m) { Id = "2" }
            };
            _mockClaimRepository.Setup(r => r.GetAll(CancellationToken.None)).ReturnsAsync(expectedClaims);

            // Act
            var result = await _claimService.GetClaimsAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockClaimRepository.Verify(r => r.GetAll(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateClaimAsync_ShouldCreateClaim()
        {
            // Arrange
            var newClaim = new Claim("cover-1", DateTime.UtcNow, "New Claim", ClaimType.BadWeather, 3000m);
            _mockClaimRepository.Setup(r => r.Create(It.IsAny<Claim>(), CancellationToken.None)).ReturnsAsync(newClaim);

            // Act
            var result = await _claimService.CreateClaimAsync(newClaim, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Claim", result.Name);
            _mockClaimRepository.Verify(r => r.Create(It.IsAny<Claim>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteClaimById_ShouldCallRepository()
        {
            // Arrange
            var claimId = "claim-1";
            _mockClaimRepository.Setup(r => r.DeleteById(claimId, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            await _claimService.DeleteClaimById(claimId, CancellationToken.None);

            // Assert
            _mockClaimRepository.Verify(r => r.DeleteById(claimId, CancellationToken.None), Times.Once);
        }
    }
}
