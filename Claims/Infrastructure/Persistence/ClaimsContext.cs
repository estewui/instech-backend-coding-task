using Microsoft.EntityFrameworkCore;

using MongoDB.EntityFrameworkCore.Extensions;

using Infrastructure.Persistence.Mongo.Models;

namespace Infrastructure.Persistence
{
    public class ClaimsContext : DbContext
    {

        private DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public ClaimsContext(DbContextOptions<ClaimsContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(CancellationToken cancellationToken = default)
        {
            return await Claims.ToListAsync(cancellationToken);
        }

        public async Task<Claim?> GetClaimAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Claims
                .Where(claim => claim.Id == id)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task AddItemAsync(Claim item, CancellationToken cancellationToken = default)
        {
            Claims.Add(item);
            await SaveChangesAsync(cancellationToken);
        }
        public async Task AddCoverAsync(Cover item, CancellationToken cancellationToken = default)
        {
            Covers.Add(item);
            await SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteItemAsync(string id, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, cancellationToken);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync(cancellationToken);
            }
        }
    }
}
