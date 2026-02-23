using Microsoft.EntityFrameworkCore;

using AutoMapper;

using Application.Abstractions.Persistence;
using DomainEntities = Domain.Entities;
using MongoModels = Infrastructure.Persistence.Mongo.Models;

namespace Infrastructure.Persistence.Mongo.Repositories
{
    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext _db;
        private readonly IMapper _mapper;

        public CoverRepository(ClaimsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<DomainEntities.Cover>> GetAll(CancellationToken cancellationToken)
        {
            var mongoCovers = await _db.Covers.ToListAsync(cancellationToken);
            return _mapper.Map<List<DomainEntities.Cover>>(mongoCovers);
        }

        public async Task<DomainEntities.Cover?> GetById(string id, CancellationToken cancellationToken)
        {
            var mongoCovers = await GetAll(cancellationToken);
            return mongoCovers.SingleOrDefault(cover => cover.Id == id);
        }

        public async Task<DomainEntities.Cover> Create(DomainEntities.Cover cover, CancellationToken cancellationToken)
        {
            var mongoCover = _mapper.Map<MongoModels.Cover>(cover);
            await _db.AddCoverAsync(mongoCover, cancellationToken);
            return cover;
        }

        public async Task DeleteById(string id, CancellationToken cancellationToken)
        {
            var mongoCovers = await _db.Covers.ToListAsync(cancellationToken);
            var mongoCover = mongoCovers.SingleOrDefault(c => c.Id == id);
            if (mongoCover is not null)
            {
                _db.Covers.Remove(mongoCover);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
