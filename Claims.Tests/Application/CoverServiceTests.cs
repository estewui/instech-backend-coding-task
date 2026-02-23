using FluentValidation;
using Moq;
using Xunit;

using Application.Abstractions.Persistence;
using Application.Services;
using Domain.Entities;

namespace Claims.Tests.Application
{
    /// <summary>
    /// Unit tests for CoverService.
    /// </summary>
    public class CoverServiceTests
    {
        private readonly Mock<ICoverRepository> _mockCoverRepository;
        private readonly Mock<IValidator<Cover>> _mockValidator;
        private readonly CoverService _coverService;

        public CoverServiceTests()
        {
            _mockCoverRepository = new Mock<ICoverRepository>();
            _mockValidator = new Mock<IValidator<Cover>>();
            _coverService = new CoverService(_mockCoverRepository.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnCover_WhenCoverExists()
        {
            // Arrange
            var coverId = "cover-1";
            var expectedCover = new Cover
            {
                Id = coverId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.Yacht,
                Premium = 5000m
            };
            _mockCoverRepository.Setup(r => r.GetById(coverId, CancellationToken.None)).ReturnsAsync(expectedCover);

            // Act
            var result = await _coverService.GetById(coverId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(coverId, result.Id);
            Assert.Equal(CoverType.Yacht, result.Type);
            _mockCoverRepository.Verify(r => r.GetById(coverId, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCovers()
        {
            // Arrange
            var expectedCovers = new List<Cover>
            {
                new Cover { Id = "1", Type = CoverType.Yacht, Premium = 1000m, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) },
                new Cover { Id = "2", Type = CoverType.Tanker, Premium = 2000m, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(60) }
            };
            _mockCoverRepository.Setup(r => r.GetAll(CancellationToken.None)).ReturnsAsync(expectedCovers);

            // Act
            var result = await _coverService.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockCoverRepository.Verify(r => r.GetAll(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCreateCover()
        {
            // Arrange
            var newCover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.PassengerShip,
                Premium = 3000m
            };
            _mockCoverRepository.Setup(r => r.Create(It.IsAny<Cover>(), CancellationToken.None)).ReturnsAsync(newCover);

            // Act
            var result = await _coverService.Create(newCover, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CoverType.PassengerShip, result.Type);
            _mockCoverRepository.Verify(r => r.Create(It.IsAny<Cover>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteById_ShouldCallRepository()
        {
            // Arrange
            var coverId = "cover-1";
            _mockCoverRepository.Setup(r => r.DeleteById(coverId, CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            await _coverService.DeleteById(coverId, CancellationToken.None);

            // Assert
            _mockCoverRepository.Verify(r => r.DeleteById(coverId, CancellationToken.None), Times.Once);
        }
    }
}
