using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Requests;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    public class EfHousingRepository : IHousingRepository
    {
        private readonly AppDbContext _db;

        public EfHousingRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.HousingListings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyList<HousingListing>> SearchAsync(
            ListingSearchCriteria criteria,
            CancellationToken ct = default)
        {
            IQueryable<HousingListing> q = _db.HousingListings.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                var city = criteria.City.Trim();
                q = q.Where(x => x.City == city);
            }

            if (criteria.OnlyActive is true)
                q = q.Where(x => x.IsActive);

            if (criteria.MinPricePerMonth.HasValue)
                q = q.Where(x => x.PricePerMonth >= criteria.MinPricePerMonth.Value);

            if (criteria.MaxPricePerMonth.HasValue)
                q = q.Where(x => x.PricePerMonth <= criteria.MaxPricePerMonth.Value);

            if (criteria.RoomType.HasValue)
                q = q.Where(x => x.RoomType == criteria.RoomType.Value);

            // Сортування за замовчуванням: новіші першими
            q = q.OrderByDescending(x => x.CreatedAtUtc);

            // paging
            var skip = (criteria.Page - 1) * criteria.PageSize;
            q = q.Skip(skip).Take(criteria.PageSize);

            return await q.ToListAsync(ct);
        }

        public Task AddAsync(HousingListing listing, CancellationToken ct = default)
            => _db.HousingListings.AddAsync(listing, ct).AsTask();

        public Task UpdateAsync(HousingListing listing, CancellationToken ct = default)
        {
            _db.HousingListings.Update(listing);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
