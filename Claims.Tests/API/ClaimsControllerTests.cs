using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;
using Moq;
using Xunit;

using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Controllers;
using API.Mapping;
using APIContractTypes = API.Contracts.Types;
using Application.Common.Auditing;
using Application.Services;
using Domain.Entities;

namespace Claims.Tests.API
{
    /// <summary>
    /// Unit tests for ClaimsController.
    /// </summary>
    public class ClaimsControllerTests
    {
        private readonly Mock<ILogger<ClaimsController>> _mockLogger;
        private readonly Mock<IClaimService> _mockClaimService;
        private readonly ClaimsController _controller;

        public ClaimsControllerTests()
        {
            _mockLogger = new Mock<ILogger<ClaimsController>>();
            _mockClaimService = new Mock<IClaimService>();
            _controller = new ClaimsController(_mockLogger.Object, _mockClaimService.Object, CreateMapper());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnAllClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("cover-1", DateTime.UtcNow, "Claim 1", ClaimType.Fire, 1000m) { Id = "1" },
                new Claim("cover-2", DateTime.UtcNow, "Claim 2", ClaimType.Collision, 2000m) { Id = "2" }
            };
            _mockClaimService.Setup(s => s.GetClaimsAsync(CancellationToken.None)).ReturnsAsync(claims);

            // Act
            var result = await _controller.GetAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            Assert.Equal("Claim 1", result.Value[0].Name);
            Assert.Equal("Claim 2", result.Value[1].Name);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnClaim_WhenClaimExists()
        {
            // Arrange
            var claimId = "claim-1";
            var claim = new Claim("cover-1", DateTime.UtcNow, "Test Claim", ClaimType.BadWeather, 3000m)
            {
                Id = claimId
            };
            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId, CancellationToken.None)).ReturnsAsync(claim);

            // Act
            var result = await _controller.GetAsync(claimId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var claimResponse = Assert.IsType<ClaimResponse>(okResult.Value);
            Assert.Equal(claimId, claimResponse.Id);
            Assert.Equal("Test Claim", claimResponse.Name);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnNotFound_WhenClaimDoesNotExist()
        {
            // Arrange
            var claimId = "non-existent";
            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId, CancellationToken.None)).ReturnsAsync((Claim?)null);

            // Act
            var result = await _controller.GetAsync(claimId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedClaim()
        {
            // Arrange
            var request = new CreateClaimRequest
            {
                CoverId = "cover-1",
                Name = "New Claim",
                Type = APIContractTypes.ClaimType.Collision,
                DamageCost = 5000m,
                Created = DateTime.UtcNow
            };
            var createdClaim = new Claim(request.CoverId, request.Created, request.Name, ClaimType.Collision, request.DamageCost)
            {
                Id = Guid.NewGuid().ToString()
            };
            _mockClaimService.Setup(s => s.CreateClaimAsync(It.IsAny<Claim>(), CancellationToken.None)).ReturnsAsync(createdClaim);

            // Act
            var result = await _controller.CreateAsync(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var claimResponse = Assert.IsType<ClaimResponse>(okResult.Value);
            Assert.Equal("New Claim", claimResponse.Name);
            Assert.Equal(5000m, claimResponse.DamageCost);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnOk()
        {
            // Arrange
            var claimId = "claim-1";

            // Act
            var result = await _controller.DeleteAsync(claimId, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockClaimService.Verify(s => s.DeleteClaimById(claimId, CancellationToken.None), Times.Once);
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }
    }
}
