using rent_for_students.Domain.Requests;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Contracts
{
    public interface IHousingRepository
    {
        Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<HousingListing>> SearchAsync(ListingSearchCriteria criteria, CancellationToken ct = default);
        Task AddAsync(HousingListing listing, CancellationToken ct = default);
        Task UpdateAsync(HousingListing listing, CancellationToken ct = default);

        // опційно для “повноти” (часто треба)
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
