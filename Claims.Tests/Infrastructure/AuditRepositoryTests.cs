using Microsoft.EntityFrameworkCore;

using AutoMapper;
using Moq;
using Xunit;

using Application.Common.Auditing;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Sql.Repositories;

namespace Claims.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for AuditRepository.
    /// </summary>
    public class AuditRepositoryTests
    {
        private static readonly DateTime FixedNow = new(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        private readonly AuditContext _context;
        private readonly AuditRepository _repository;
        private readonly Mock<IMapper> _mockMapper;

        public AuditRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AuditContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AuditContext(options);
            _mockMapper = new Mock<IMapper>();
            _repository = new AuditRepository(_context, _mockMapper.Object);
        }

        [Fact]
        public async Task AuditClaim_ShouldAddClaimAuditToDatabase()
        {
            // Arrange
            var claimId = "claim-1";
            var httpRequestType = "POST";

            // Act
            await _repository.AuditClaim(new AuditEvent { EntityId = claimId, HttpRequestType = httpRequestType, Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audits = _context.ClaimAudits.ToList();
            Assert.Single(audits);
            Assert.Equal(claimId, audits[0].ClaimId);
            Assert.Equal(httpRequestType, audits[0].HttpRequestType);
            Assert.Equal(FixedNow, audits[0].Created);
        }

        [Fact]
        public async Task AuditCover_ShouldAddCoverAuditToDatabase()
        {
            // Arrange
            var coverId = "cover-1";
            var httpRequestType = "DELETE";

            // Act
            await _repository.AuditCover(new AuditEvent { EntityId = coverId, HttpRequestType = httpRequestType, Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audits = _context.CoverAudits.ToList();
            Assert.Single(audits);
            Assert.Equal(coverId, audits[0].CoverId);
            Assert.Equal(httpRequestType, audits[0].HttpRequestType);
            Assert.Equal(FixedNow, audits[0].Created);
        }

        [Fact]
        public async Task AuditClaim_ShouldAddMultipleAudits()
        {
            // Arrange
            var claimId1 = "claim-1";
            var claimId2 = "claim-2";

            // Act
            await _repository.AuditClaim(new AuditEvent { EntityId = claimId1, HttpRequestType = "POST", Timestamp = FixedNow }, CancellationToken.None);
            await _repository.AuditClaim(new AuditEvent { EntityId = claimId2, HttpRequestType = "DELETE", Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audits = _context.ClaimAudits.ToList();
            Assert.Equal(2, audits.Count);
        }

        [Fact]
        public async Task AuditCover_ShouldAddMultipleAudits()
        {
            // Arrange
            var coverId1 = "cover-1";
            var coverId2 = "cover-2";

            // Act
            await _repository.AuditCover(new AuditEvent { EntityId = coverId1, HttpRequestType = "POST", Timestamp = FixedNow }, CancellationToken.None);
            await _repository.AuditCover(new AuditEvent { EntityId = coverId2, HttpRequestType = "DELETE", Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audits = _context.CoverAudits.ToList();
            Assert.Equal(2, audits.Count);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("DELETE")]
        [InlineData("PUT")]
        [InlineData("GET")]
        public async Task AuditClaim_ShouldSupportDifferentHttpMethods(string httpMethod)
        {
            // Arrange
            var claimId = "claim-1";

            // Act
            await _repository.AuditClaim(new AuditEvent { EntityId = claimId, HttpRequestType = httpMethod, Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audit = _context.ClaimAudits.First();
            Assert.Equal(httpMethod, audit.HttpRequestType);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("DELETE")]
        [InlineData("PUT")]
        [InlineData("GET")]
        public async Task AuditCover_ShouldSupportDifferentHttpMethods(string httpMethod)
        {
            // Arrange
            var coverId = "cover-1";

            // Act
            await _repository.AuditCover(new AuditEvent { EntityId = coverId, HttpRequestType = httpMethod, Timestamp = FixedNow }, CancellationToken.None);

            // Assert
            var audit = _context.CoverAudits.First();
            Assert.Equal(httpMethod, audit.HttpRequestType);
        }
    }
}
