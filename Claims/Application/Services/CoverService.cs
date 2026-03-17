using Application.Common.Auditing;
using FluentValidation;

using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Services;

namespace Application.Services
{
    public class CoverService : ICoverService
    {
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditSink _auditSink;
        private readonly IValidator<Cover> _validator;

        public CoverService(ICoverRepository coverRepository, IAuditSink auditSink, IValidator<Cover> validator)
        {
            _coverRepository = coverRepository;
            _auditSink = auditSink;
            _validator = validator;
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return PremiumCalculator.ComputePremium(startDate, endDate, coverType);
        }

        public Task<Cover?> GetById(string id, CancellationToken cancellationToken)
        {
            return _coverRepository.GetById(id, cancellationToken);
        }

        public async Task<IEnumerable<Cover>> GetAll(CancellationToken cancellationToken)
        {
            return await _coverRepository.GetAll(cancellationToken);
        }

        public async Task<Cover> Create(Cover cover, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(cover);

            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            var created = await _coverRepository.Create(cover, cancellationToken);

            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Cover,
                EntityId = created.Id,
                HttpRequestType = "POST"
            }, cancellationToken);

            return created;
        }

        public async Task DeleteById(string id, CancellationToken cancellationToken)
        {
            await _coverRepository.DeleteById(id, cancellationToken);

            await _auditSink.EnqueueAsync(new AuditEvent
            {
                Type = AuditType.Cover,
                EntityId = id,
                HttpRequestType = "DELETE"
            }, cancellationToken);
        }
    }
}
