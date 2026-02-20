using Application.Common.Auditing;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace Claims.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for AuditRepository.
    /// </summary>
    public class AuditRepositoryTests
    {
        private readonly AuditContext _context;
        private readonly AuditRepository _repository;

        public AuditRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AuditContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AuditContext(options);
            _repository = new AuditRepository(_context);
        }

        [Fact]
        public void AuditClaim_ShouldAddClaimAuditToDatabase()
        {
            // Arrange
            var claimId = "claim-1";
            var httpRequestType = "POST";

            // Act
            _repository.AuditClaim(new AuditEvent { EntityId = claimId, HttpRequestType = httpRequestType, Timestamp = DateTime.Now });

            // Assert
            var audits = _context.ClaimAudits.ToList();
            Assert.Single(audits);
            Assert.Equal(claimId, audits[0].ClaimId);
            Assert.Equal(httpRequestType, audits[0].HttpRequestType);
            Assert.True(audits[0].Created <= DateTime.Now);
        }

        [Fact]
        public void AuditCover_ShouldAddCoverAuditToDatabase()
        {
            // Arrange
            var coverId = "cover-1";
            var httpRequestType = "DELETE";

            // Act
            _repository.AuditCover(new AuditEvent { EntityId = coverId, HttpRequestType = httpRequestType, Timestamp = DateTime.Now });

            // Assert
            var audits = _context.CoverAudits.ToList();
            Assert.Single(audits);
            Assert.Equal(coverId, audits[0].CoverId);
            Assert.Equal(httpRequestType, audits[0].HttpRequestType);
            Assert.True(audits[0].Created <= DateTime.Now);
        }

        [Fact]
        public void AuditClaim_ShouldAddMultipleAudits()
        {
            // Arrange
            var claimId1 = "claim-1";
            var claimId2 = "claim-2";

            // Act
            _repository.AuditClaim(new AuditEvent { EntityId = claimId1, HttpRequestType = "POST", Timestamp = DateTime.Now });
            _repository.AuditClaim(new AuditEvent { EntityId = claimId2, HttpRequestType = "DELETE", Timestamp = DateTime.Now });

            // Assert
            var audits = _context.ClaimAudits.ToList();
            Assert.Equal(2, audits.Count);
        }

        [Fact]
        public void AuditCover_ShouldAddMultipleAudits()
        {
            // Arrange
            var coverId1 = "cover-1";
            var coverId2 = "cover-2";

            // Act
            _repository.AuditCover(new AuditEvent { EntityId = coverId1, HttpRequestType = "POST", Timestamp = DateTime.Now });
            _repository.AuditCover(new AuditEvent { EntityId = coverId2, HttpRequestType = "DELETE", Timestamp = DateTime.Now });

            // Assert
            var audits = _context.CoverAudits.ToList();
            Assert.Equal(2, audits.Count);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("DELETE")]
        [InlineData("PUT")]
        [InlineData("GET")]
        public void AuditClaim_ShouldSupportDifferentHttpMethods(string httpMethod)
        {
            // Arrange
            var claimId = "claim-1";

            // Act
            _repository.AuditClaim(new AuditEvent { EntityId = claimId, HttpRequestType = httpMethod, Timestamp = DateTime.Now });

            // Assert
            var audit = _context.ClaimAudits.First();
            Assert.Equal(httpMethod, audit.HttpRequestType);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("DELETE")]
        [InlineData("PUT")]
        [InlineData("GET")]
        public void AuditCover_ShouldSupportDifferentHttpMethods(string httpMethod)
        {
            // Arrange
            var coverId = "cover-1";

            // Act
            _repository.AuditCover(new AuditEvent { EntityId = coverId, HttpRequestType = httpMethod, Timestamp = DateTime.Now });

            // Assert
            var audit = _context.CoverAudits.First();
            Assert.Equal(httpMethod, audit.HttpRequestType);
        }
    }
}
