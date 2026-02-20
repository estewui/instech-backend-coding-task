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
        private readonly Mock<IAuditSink> _mockAuditSink;
        private readonly ClaimsController _controller;

        public ClaimsControllerTests()
        {
            _mockLogger = new Mock<ILogger<ClaimsController>>();
            _mockClaimService = new Mock<IClaimService>();
            _mockAuditSink = new Mock<IAuditSink>();
            _controller = new ClaimsController(_mockLogger.Object, _mockClaimService.Object, _mockAuditSink.Object, CreateMapper());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnAllClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim { Id = "1", Name = "Claim 1", CoverId = "cover-1", Type = ClaimType.Fire, DamageCost = 1000m, Created = DateTime.UtcNow },
                new Claim { Id = "2", Name = "Claim 2", CoverId = "cover-2", Type = ClaimType.Collision, DamageCost = 2000m, Created = DateTime.UtcNow }
            };
            _mockClaimService.Setup(s => s.GetClaimsAsync()).ReturnsAsync(claims);

            // Act
            var result = await _controller.GetAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Claim 1", result[0].Name);
            Assert.Equal("Claim 2", result[1].Name);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnClaim_WhenClaimExists()
        {
            // Arrange
            var claimId = "claim-1";
            var claim = new Claim
            {
                Id = claimId,
                Name = "Test Claim",
                CoverId = "cover-1",
                Type = ClaimType.BadWeather,
                DamageCost = 3000m,
                Created = DateTime.UtcNow
            };
            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId)).ReturnsAsync(claim);

            // Act
            var result = await _controller.GetAsync(claimId);

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
            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId)).ReturnsAsync((Claim?)null);

            // Act
            var result = await _controller.GetAsync(claimId);

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
            var createdClaim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = request.CoverId,
                Name = request.Name,
                Type = ClaimType.Collision,
                DamageCost = request.DamageCost,
                Created = request.Created
            };
            _mockClaimService.Setup(s => s.CreateClaimAsync(It.IsAny<Claim>())).ReturnsAsync(createdClaim);

            // Act
            var result = await _controller.CreateAsync(request);

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
            var result = await _controller.DeleteAsync(claimId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockClaimService.Verify(s => s.DeleteClaimById(claimId), Times.Once);
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }
    }
}
