using FluentValidation;

using Application.Common.Auditing;
using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditSink _auditSink;

        public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditSink auditSink)
        {
            _claimRepository = claimRepository;
            _coverRepository = coverRepository;
            _auditSink = auditSink;
        }

        public async Task<Claim?> GetClaimByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _claimRepository.GetById(id, cancellationToken);
        }

        public async Task DeleteClaimById(string id, CancellationToken cancellationToken)
        {
            await _claimRepository.DeleteById(id, cancellationToken);

            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Claim,
                EntityId = id,
                HttpRequestType = "DELETE",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task<Claim> CreateClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            var cover = await _coverRepository.GetById(claim.CoverId, cancellationToken);
            if (cover is null)
                throw new InvalidOperationException("Cover not found.");

            if (claim.Created < cover.StartDate || claim.Created > cover.EndDate)
                throw new InvalidOperationException("Created date must be within the period of the related Cover.");

            var created = await _claimRepository.Create(claim, cancellationToken);

            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Claim,
                EntityId = created.Id,
                HttpRequestType = "POST",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return created;
        }

        public async Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
        {
            return await _claimRepository.GetAll(cancellationToken);
        }
    }
}