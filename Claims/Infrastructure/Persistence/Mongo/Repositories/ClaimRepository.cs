using Application.Abstractions.Persistence;
using AutoMapper;
using DomainEntities = Domain.Entities;
using MongoModels = Infrastructure.Persistence.Mongo.Models;

namespace Infrastructure.Persistence.Mongo.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext _db;
        private readonly IMapper _mapper;

        public ClaimRepository(ClaimsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<DomainEntities.Claim>> GetAll()
        {
            var mongoClaims = await _db.GetClaimsAsync();
            return _mapper.Map<List<DomainEntities.Claim>>(mongoClaims.ToList());
        }

        public async Task<DomainEntities.Claim?> GetById(string id)
        {
            var mongoClaim = await _db.GetClaimAsync(id);
            return _mapper.Map<DomainEntities.Claim?>(mongoClaim);
        }

        public async Task<DomainEntities.Claim> Create(DomainEntities.Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            var mongoClaim = _mapper.Map<MongoModels.Claim>(claim);
            await _db.AddItemAsync(mongoClaim);
            return claim;
        }


        public async Task DeleteById(string id)
        {
            await _db.DeleteItemAsync(id);
        }
    }
}
