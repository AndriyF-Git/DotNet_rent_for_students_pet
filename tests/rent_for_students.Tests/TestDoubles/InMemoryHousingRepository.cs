using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Requests;

namespace rent_for_students.Tests.TestDoubles
{
    internal sealed class InMemoryHousingRepository : IHousingRepository
    {
        private readonly List<HousingListing> _items = new();

        public Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<HousingListing>> SearchAsync(ListingSearchCriteria criteria, CancellationToken ct = default)
        {
            IEnumerable<HousingListing> query = _items;

            if (!string.IsNullOrWhiteSpace(criteria.City))
            {
                query = query.Where(x => x.City == criteria.City);
            }

            if (criteria.OnlyActive == true)
            {
                query = query.Where(x => x.IsActive);
            }

            return Task.FromResult<IReadOnlyList<HousingListing>>(query.ToList());
        }

        public Task AddAsync(HousingListing listing, CancellationToken ct = default)
        {
            _items.Add(listing);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(HousingListing listing, CancellationToken ct = default)
        {
            var idx = _items.FindIndex(x => x.Id == listing.Id);
            if (idx >= 0)
            {
                _items[idx] = listing;
            }

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
