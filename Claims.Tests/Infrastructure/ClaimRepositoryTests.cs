using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Claims.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for ClaimRepository.
    /// </summary>
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
            _repository = new ClaimRepository(_context);
        }

        [Fact]
        public async Task Create_ShouldAddClaimToDatabase()
        {
            // Arrange
            var claim = new Claim
            {
                CoverId = "cover-1",
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 5000m,
                Created = DateTime.UtcNow
            };

            // Act
            var result = await _repository.Create(claim);

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
            var claim = new Claim
            {
                CoverId = "cover-1",
                Name = "Test Claim",
                Type = ClaimType.Fire,
                DamageCost = 3000m,
                Created = DateTime.UtcNow
            };
            var createdClaim = await _repository.Create(claim);

            // Act
            var result = await _repository.GetById(createdClaim.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdClaim.Id, result.Id);
            Assert.Equal("Test Claim", result.Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllClaims()
        {
            // Arrange
            var claim1 = new Claim { CoverId = "cover-1", Name = "Claim 1", Type = ClaimType.Grounding, DamageCost = 1000m, Created = DateTime.UtcNow };
            var claim2 = new Claim { CoverId = "cover-2", Name = "Claim 2", Type = ClaimType.BadWeather, DamageCost = 2000m, Created = DateTime.UtcNow };
            await _repository.Create(claim1);
            await _repository.Create(claim2);

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteById_ShouldRemoveClaimFromDatabase()
        {
            // Arrange
            var claim = new Claim
            {
                CoverId = "cover-1",
                Name = "Test Claim",
                Type = ClaimType.Collision,
                DamageCost = 4000m,
                Created = DateTime.UtcNow
            };
            var createdClaim = await _repository.Create(claim);

            // Act
            await _repository.DeleteById(createdClaim.Id);
            var result = await _repository.GetById(createdClaim.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteById_ShouldNotThrow_WhenClaimDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();

            // Act & Assert
            await _repository.DeleteById(nonExistentId); // Should not throw
        }
    }
}
