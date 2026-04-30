using rent_for_students.Application.Common;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Reports;
using rent_for_students.Domain.Requests;

namespace rent_for_students.Application.UseCases
{
    public interface IListingUseCaseMediator
    {
        Task<Result<IReadOnlyList<HousingListing>>> SearchListingsAsync(ListingSearchCriteria criteria, CancellationToken ct = default);
        Task<Result<HousingListing>> GetDetailsAsync(Guid id, CancellationToken ct = default);
        Task<Result<Guid>> CreateAsync(HousingListing listing, CancellationToken ct = default);
        Task<Result<Guid>> CreateDraftAsync(HousingListing draft, CancellationToken ct = default);
        Task<Result<bool>> UpdateDraftAsync(Guid id, HousingListing draft, CancellationToken ct = default);
        Task<Result<bool>> PublishAsync(Guid id, CancellationToken ct = default);
        Task<Result<bool>> UpdateAsync(Guid id, HousingListing listing, CancellationToken ct = default);
        Task<Result<IReadOnlyList<ListingDemandReportRow>>> GetDemandReportAsync(CancellationToken ct = default);
    }
}
