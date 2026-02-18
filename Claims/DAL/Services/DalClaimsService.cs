using DAL.Data;
using DAL.Models;
using Business.Models;

namespace DAL.Services
{
    public class DalClaimsService : IDalClaimsService
    {
        private readonly ClaimsContext _claimsContext;

        public DalClaimsService(ClaimsContext claimsContext)
        {
            _claimsContext = claimsContext;
        }
        public async Task<List<Claim>> GetAll()
        {
            var claims = await _claimsContext.GetClaimsAsync();
            return claims.Select(claimDb => new Claim
            {
                Id = claimDb.Id,
                CoverId = claimDb.CoverId,
                Created = claimDb.Created,
                Name = claimDb.Name,
                Type = (Business.Models.ClaimType)claimDb.Type,
                DamageCost = claimDb.DamageCost
            }).ToList();
        }
        public async Task<Claim> GetById(string id)
        {
            var claim = await _claimsContext.GetClaimAsync(id);
            return new Claim
            {
                Id = claim.Id,
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = (Business.Models.ClaimType)claim.Type,
                DamageCost = claim.DamageCost
            };
        }

        public async Task<Claim> Create(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            var claimDb = new ClaimDb
            {
                Id = claim.Id,
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = (DAL.Models.ClaimType)claim.Type,
                DamageCost = claim.DamageCost
            };
            await _claimsContext.AddItemAsync(claimDb);
            return new Claim
            {
                Id = claimDb.Id,
                CoverId = claimDb.CoverId,
                Created = claimDb.Created,
                Name = claimDb.Name,
                Type = (Business.Models.ClaimType)claimDb.Type,
                DamageCost = claimDb.DamageCost
            };
        }

        public async Task DeleteById(string id)
        {
            await _claimsContext.DeleteItemAsync(id);
        }
    }
}
