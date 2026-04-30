using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Requests;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Domain.Services
{
    public class HousingService
    {
        private readonly IHousingRepository _repository;

        public HousingService(IHousingRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<HousingListing>> SearchListingsAsync(
            ListingSearchCriteria criteria,
            CancellationToken ct = default)
        {
            // Тут може бути бізнес-логіка (нормалізація, дефолти, правила)
            if (criteria.Page <= 0) criteria.Page = 1;
            if (criteria.PageSize is < 1 or > 200) criteria.PageSize = 20;

            return _repository.SearchAsync(criteria, ct);
        }

        public Task<HousingListing?> GetListingDetailsAsync(Guid id, CancellationToken ct = default)
            => _repository.GetByIdAsync(id, ct);

        public async Task<Guid> CreateListingAsync(HousingListing listing, CancellationToken ct = default)
        {
            // Базові бізнес-запобіжники (мінімум)
            listing.Id = listing.Id == Guid.Empty ? Guid.NewGuid() : listing.Id;
            listing.CreatedAtUtc = listing.CreatedAtUtc == default ? DateTime.UtcNow : listing.CreatedAtUtc;
            listing.IsActive = true;

            await _repository.AddAsync(listing, ct);
            await _repository.SaveChangesAsync(ct);

            return listing.Id;
        }

        public async Task<Guid> CreateListingDraftAsync(HousingListing draft, CancellationToken ct = default)
        {
            draft.Id = draft.Id == Guid.Empty ? Guid.NewGuid() : draft.Id;
            draft.IsActive = false;

            await _repository.AddAsync(draft, ct);
            await _repository.SaveChangesAsync(ct);

            return draft.Id;
        }

        public async Task<bool> UpdateListingDraftAsync(Guid id, HousingListing draft, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null || existing.IsActive)
            {
                return false;
            }

            MapEditableFields(existing, draft);
            existing.IsActive = false;

            await _repository.UpdateAsync(existing, ct);
            await _repository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> PublishListingAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return false;
            }

            existing.IsActive = true;
            existing.CreatedAtUtc = DateTime.UtcNow;

            await _repository.UpdateAsync(existing, ct);
            await _repository.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> UpdateListingAsync(Guid id, HousingListing listing, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                return false;
            }

            MapEditableFields(existing, listing);

            await _repository.UpdateAsync(existing, ct);
            await _repository.SaveChangesAsync(ct);

            return true;
        }

        private static void MapEditableFields(HousingListing target, HousingListing source)
        {
            target.Title = source.Title;
            target.Description = source.Description;
            target.City = source.City;
            target.District = source.District;
            target.PricePerMonth = source.PricePerMonth;
            target.RoomType = source.RoomType;
            target.AreaSqm = source.AreaSqm;
        }
    }
}
