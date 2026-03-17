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
        private readonly IValidator<Claim> _validator;

        public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditSink auditSink, IValidator<Claim> validator)
        {
            _claimRepository = claimRepository;
            _coverRepository = coverRepository;
            _auditSink = auditSink;
            _validator = validator;
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
                HttpRequestType = "DELETE"
            }, cancellationToken);
        }

        public async Task<Claim> CreateClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(claim);

            var created = await _claimRepository.Create(claim, cancellationToken);

            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Claim,
                EntityId = created.Id,
                HttpRequestType = "POST"
            }, cancellationToken);

            return created;
        }

        public async Task<List<Claim>> GetClaimsAsync(CancellationToken cancellationToken)
        {
            return await _claimRepository.GetAll(cancellationToken);
        }
    }
}