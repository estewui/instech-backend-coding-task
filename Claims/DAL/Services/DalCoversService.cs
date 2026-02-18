using Business.Models;
using DAL.Data;
using DAL.Models;
using MongoDB.Driver.Linq;


namespace DAL.Services
{
    public class DalCoversService : IDalCoversService
    {
        private readonly ClaimsContext _claimsContext;

        public DalCoversService(ClaimsContext claimsContext)
        {
            _claimsContext = claimsContext;
        }

        public async Task<List<Cover>> GetAll()
        {
            var covers = await _claimsContext.Covers.ToListAsync();
            return covers.Select(c => new Cover { Id = c.Id, StartDate = c.StartDate, EndDate = c.EndDate, Type = (Business.Models.CoverType)c.Type, Premium = c.Premium }).ToList();
        }
        public async Task<Cover> GetById(string id)
        {
            var covers = await GetAll();
            return covers.SingleOrDefault(cover => cover.Id == id);
        }

        public Cover Create(Cover cover)
        {
            var coverDb = new CoverDb { Id = cover.Id, StartDate = cover.StartDate, EndDate = cover.EndDate, Type = (DAL.Models.CoverType)cover.Type, Premium = cover.Premium };
            _claimsContext.Covers.Add(coverDb);
            return new Cover
            {
                Id = coverDb.Id,
                StartDate = coverDb.StartDate,
                EndDate = coverDb.EndDate,
                Type = (Business.Models.CoverType)coverDb.Type,
                Premium = coverDb.Premium
            };
        }

        public async Task DeleteById(string id)
        {
            var cover = await _claimsContext.Covers.Where(cover => cover.Id == id).SingleOrDefaultAsync();
            if (cover is not null)
            {
                _claimsContext.Covers.Remove(cover);
                await _claimsContext.SaveChangesAsync();
            }
        }

    }
}
