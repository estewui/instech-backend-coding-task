using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CoverRepository: ICoverRepository
    {
        private readonly ClaimsContext _db;

        public CoverRepository(ClaimsContext db)
        {
            _db = db;
        }

        public async Task<List<Cover>> GetAll()
        {
            var covers = await _db.Covers.ToListAsync();
            return covers;
        }
        public async Task<Cover> GetById(string id)
        {
            var covers = await GetAll();
            return covers.SingleOrDefault(cover => cover.Id == id);
        }

        public Cover Create(Cover cover)
        {
            _db.Covers.Add(cover);
            return cover;
        }

        public async Task DeleteById(string id)
        {
            var cover = await _db.Covers.Where(cover => cover.Id == id).SingleOrDefaultAsync();
            if (cover is not null)
            {
                _db.Covers.Remove(cover);
                await _db.SaveChangesAsync();
            }
        }
    }
}
