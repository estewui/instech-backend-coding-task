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

        public async Task<List<DomainEntities.Cover>> GetAll()
        {
            var mongoCovers = await _db.Covers.ToListAsync();
            return _mapper.Map<List<DomainEntities.Cover>>(mongoCovers);
        }

        public async Task<DomainEntities.Cover?> GetById(string id)
        {
            var mongoCovers = await GetAll();
            return mongoCovers.SingleOrDefault(cover => cover.Id == id);
        }

        public async Task<DomainEntities.Cover> Create(DomainEntities.Cover cover)
        {
            var mongoCover = _mapper.Map<MongoModels.Cover>(cover);
            await _db.AddCoverAsync(mongoCover);
            return cover;
        }

        public async Task DeleteById(string id)
        {
            var mongoCovers = await _db.Covers.ToListAsync();
            var mongoCover = mongoCovers.SingleOrDefault(c => c.Id == id);
            if (mongoCover is not null)
            {
                _db.Covers.Remove(mongoCover);
                await _db.SaveChangesAsync();
            }
        }
    }
}
