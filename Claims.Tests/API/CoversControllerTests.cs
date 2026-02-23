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
using Domain.Services;

namespace Claims.Tests.API
{
    /// <summary>
    /// Unit tests for CoversController.
    /// </summary>
    public class CoversControllerTests
    {
        private readonly Mock<ILogger<CoversController>> _mockLogger;
        private readonly Mock<ICoverService> _mockCoverService;
        private readonly Mock<IAuditSink> _mockAuditSink;
        private readonly CoversController _controller;

        public CoversControllerTests()
        {
            _mockLogger = new Mock<ILogger<CoversController>>();
            _mockCoverService = new Mock<ICoverService>();
            _mockAuditSink = new Mock<IAuditSink>();
            _controller = new CoversController(_mockLogger.Object, _mockCoverService.Object, _mockAuditSink.Object, CreateMapper());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnAllCovers()
        {
            // Arrange
            var covers = new List<Cover>
            {
                new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.Yacht, 1000m) { Id = "1" },
                new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(60), CoverType.Tanker, 2000m) { Id = "2" }
            };
            _mockCoverService.Setup(s => s.GetAll(CancellationToken.None)).ReturnsAsync(covers);

            // Act
            var result = await _controller.GetAsync(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var coverResponses = Assert.IsAssignableFrom<IEnumerable<CoverResponse>>(okResult.Value);
            Assert.Equal(2, coverResponses.Count());
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnCover_WhenCoverExists()
        {
            // Arrange
            var coverId = "cover-1";
            var cover = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.PassengerShip, 5000m)
            {
                Id = coverId
            };
            _mockCoverService.Setup(s => s.GetById(coverId, CancellationToken.None)).ReturnsAsync(cover);

            // Act
            var result = await _controller.GetAsync(coverId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var coverResponse = Assert.IsType<CoverResponse>(okResult.Value);
            Assert.Equal(coverId, coverResponse.Id);
            Assert.Equal(5000m, coverResponse.Premium);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldReturnNotFound_WhenCoverDoesNotExist()
        {
            // Arrange
            var coverId = "non-existent";
            _mockCoverService.Setup(s => s.GetById(coverId, CancellationToken.None)).ReturnsAsync((Cover?)null);

            // Act
            var result = await _controller.GetAsync(coverId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedCover()
        {
            // Arrange
            var request = new CreateCoverRequest
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = APIContractTypes.CoverType.Yacht,
                Premium = 0 // Will be computed
            };
            var computedPremium = 41250m;
            
            _mockCoverService.Setup(s => s.Create(It.IsAny<Cover>(), CancellationToken.None)).ReturnsAsync((Cover cover, CancellationToken ct) =>
            {
                cover.Id = "new-cover-id";
                cover.Premium = computedPremium;
                return cover;
            });

            // Act
            var result = await _controller.CreateAsync(request, CancellationToken.None);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var coverResponse = Assert.IsType<CoverResponse>(okResult.Value);
            Assert.Equal(computedPremium, coverResponse.Premium);
            _mockCoverService.Verify(s => s.Create(It.IsAny<Cover>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNoContent()
        {
            // Arrange
            var coverId = "cover-1";
            _mockCoverService.Setup(s => s.DeleteById(coverId, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync(coverId, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockCoverService.Verify(s => s.DeleteById(coverId, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ComputePremium_ShouldReturnPremium()
        {
            // Arrange
            var request = new ComputePremiumRequest
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = APIContractTypes.CoverType.Yacht
            };
            var expectedPremium = 41250m;
            _mockCoverService.Setup(s => s.ComputePremium(request.StartDate, request.EndDate, It.IsAny<CoverType>()))
                .Returns(expectedPremium);

            // Act
            var result = _controller.ComputePremium(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedPremium, okResult.Value);
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            return config.CreateMapper();
        }
    }
}
