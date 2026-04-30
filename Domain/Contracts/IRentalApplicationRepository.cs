using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Contracts
{
    public interface IRentalApplicationRepository
    {
        Task AddAsync(RentalApplication application, CancellationToken ct = default);
        Task<RentalApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<RentalApplication>> ListByListingIdAsync(Guid listingId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
