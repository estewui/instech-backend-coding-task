using Microsoft.EntityFrameworkCore;

using AutoMapper;
using Xunit;

using API.Mapping;
using Domain.Entities;
using Infrastructure.Mapping;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mongo.Repositories;

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
            _repository = new CoverRepository(_context, CreateMapper());
        }

        [Fact]
        public async Task Create_ShouldAddCoverToDatabase()
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.Yacht, 5000m)
            {
                Id = Guid.NewGuid().ToString()
            };

            // Act
            var result = await _repository.Create(cover, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cover.Id, result.Id);
            Assert.Equal(CoverType.Yacht, result.Type);
        }

        [Fact]
        public async Task GetById_ShouldReturnCover_WhenCoverExists()
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(60), CoverType.PassengerShip, 8000m)
            {
                Id = Guid.NewGuid().ToString()
            };
            await _repository.Create(cover, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _repository.GetById(cover.Id, CancellationToken.None);

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
            var result = await _repository.GetById(nonExistentId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCovers()
        {
            // Arrange
            var cover1 = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.Yacht, 1000m) { Id = Guid.NewGuid().ToString() };
            var cover2 = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(60), CoverType.Tanker, 2000m) { Id = Guid.NewGuid().ToString() };
            await _repository.Create(cover1, CancellationToken.None);
            await _repository.Create(cover2, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _repository.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveCoverFromDatabase()
        {
            // Arrange
            var cover = new Cover(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), CoverType.ContainerShip, 6000m)
            {
                Id = Guid.NewGuid().ToString()
            };
            await _repository.Create(cover, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _repository.DeleteById(cover.Id, CancellationToken.None);
            var result = await _repository.GetById(cover.Id, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteById_ShouldNotThrow_WhenCoverDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();

            // Act & Assert
            await _repository.DeleteById(nonExistentId, CancellationToken.None); // Should not throw
        }

        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InfrastructureMappingProfile>();
                cfg.AddProfile<MappingProfile>();
            });
            return config.CreateMapper();
        }
    }
}
