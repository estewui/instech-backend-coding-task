using Microsoft.EntityFrameworkCore;

using AutoMapper;
using Moq;
using Xunit;

using API.Mapping;
using Domain.Entities;
using Infrastructure.Mapping;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mongo.Repositories;

namespace Claims.Tests.Infrastructure
{
    public class ClaimRepositoryTests
    {
        private readonly ClaimsContext _context;
        private readonly ClaimRepository _repository;

        public ClaimRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ClaimsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ClaimsContext(options);
            _repository = new ClaimRepository(_context, CreateMapper());
        }

        [Fact]
        public async Task Create_ShouldAddClaimToDatabase()
        {
            // Arrange
            var claim = new Claim("cover-1", DateTime.UtcNow, "Test Claim", ClaimType.Collision, 5000m);

            // Act
            var result = await _repository.Create(claim, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.NotEmpty(result.Id);
            Assert.Equal("Test Claim", result.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnClaim_WhenClaimExists()
        {
            // Arrange
            var claim = new Claim("cover-1", DateTime.UtcNow, "Test Claim", ClaimType.Fire, 3000m);
            var createdClaim = await _repository.Create(claim, CancellationToken.None);

            // Act
            var result = await _repository.GetById(createdClaim.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdClaim.Id, result.Id);
            Assert.Equal("Test Claim", result.Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllClaims()
        {
            // Arrange
            var claim1 = new Claim("cover-1", DateTime.UtcNow, "Claim 1", ClaimType.Grounding, 1000m);
            var claim2 = new Claim("cover-2", DateTime.UtcNow, "Claim 2", ClaimType.BadWeather, 2000m);
            await _repository.Create(claim1, CancellationToken.None);
            await _repository.Create(claim2, CancellationToken.None);

            // Act
            var result = await _repository.GetAll(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveClaimFromDatabase()
        {
            // Arrange
            var claim = new Claim("cover-1", DateTime.UtcNow, "Test Claim", ClaimType.Collision, 4000m);
            var createdClaim = await _repository.Create(claim, CancellationToken.None);

            // Act
            await _repository.DeleteById(createdClaim.Id, CancellationToken.None);
            var result = await _repository.GetById(createdClaim.Id, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteById_ShouldNotThrow_WhenClaimDoesNotExist()
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
