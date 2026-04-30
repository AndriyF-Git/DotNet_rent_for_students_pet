using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    public class EfRentalApplicationRepository : IRentalApplicationRepository
    {
        private readonly AppDbContext _db;

        public EfRentalApplicationRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(RentalApplication application, CancellationToken ct = default)
            => _db.RentalApplications.AddAsync(application, ct).AsTask();

        public Task<RentalApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.RentalApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyList<RentalApplication>> ListByListingIdAsync(Guid listingId, CancellationToken ct = default)
            => await _db.RentalApplications
                .AsNoTracking()
                .Where(x => x.ListingId == listingId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
