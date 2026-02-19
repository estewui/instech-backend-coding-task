using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class ClaimRepository: IClaimRepository
    {
        private readonly ClaimsContext _db;

        public ClaimRepository(ClaimsContext db)
        {
            _db = db;
        }
        public async Task<List<Claim>> GetAll()
        {
            var claims = await _db.GetClaimsAsync();
            return claims.ToList();
        }
        public async Task<Claim> GetById(string id)
        {
            var claim = await _db.GetClaimAsync(id);
            return claim;
        }

        public async Task<Claim> Create(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _db.AddItemAsync(claim);
            return claim;
        }

        public async Task DeleteById(string id)
        {
            await _db.DeleteItemAsync(id);
        }
    }
}
