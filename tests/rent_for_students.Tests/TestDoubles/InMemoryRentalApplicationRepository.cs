using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Tests.TestDoubles
{
    internal sealed class InMemoryRentalApplicationRepository : IRentalApplicationRepository
    {
        private readonly List<RentalApplication> _items = new();

        public Task AddAsync(RentalApplication application, CancellationToken ct = default)
        {
            _items.Add(application);
            return Task.CompletedTask;
        }

        public Task<RentalApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<RentalApplication>> ListByListingIdAsync(Guid listingId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<RentalApplication>>(
                _items.Where(x => x.ListingId == listingId).OrderByDescending(x => x.CreatedAtUtc).ToList());

        public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
