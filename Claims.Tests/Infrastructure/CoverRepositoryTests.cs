using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Claims.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for CoverRepository.
    /// </summary>
    public class CoverRepositoryTests
    {
        private readonly ClaimsContext _context;
        private readonly CoverRepository _repository;

        public CoverRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ClaimsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ClaimsContext(options);
            _repository = new CoverRepository(_context);
        }

        [Fact]
        public async Task Create_ShouldAddCoverToDatabase()
        {
            // Arrange
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.Yacht,
                Premium = 5000m
            };

            // Act
            var result = await _repository.Create(cover);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cover.Id, result.Id);
            Assert.Equal(CoverType.Yacht, result.Type);
        }

        [Fact]
        public async Task GetById_ShouldReturnCover_WhenCoverExists()
        {
            // Arrange
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(60),
                Type = CoverType.PassengerShip,
                Premium = 8000m
            };
            _repository.Create(cover);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetById(cover.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cover.Id, result.Id);
            Assert.Equal(CoverType.PassengerShip, result.Type);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCoverDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();

            // Act
            var result = await _repository.GetById(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCovers()
        {
            // Arrange
            var cover1 = new Cover { Id = Guid.NewGuid().ToString(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Type = CoverType.Yacht, Premium = 1000m };
            var cover2 = new Cover { Id = Guid.NewGuid().ToString(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(60), Type = CoverType.Tanker, Premium = 2000m };
            _repository.Create(cover1);
            _repository.Create(cover2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveCoverFromDatabase()
        {
            // Arrange
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.ContainerShip,
                Premium = 6000m
            };
            _repository.Create(cover);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteById(cover.Id);
            var result = await _repository.GetById(cover.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteById_ShouldNotThrow_WhenCoverDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();

            // Act & Assert
            await _repository.DeleteById(nonExistentId); // Should not throw
        }
    }
}
