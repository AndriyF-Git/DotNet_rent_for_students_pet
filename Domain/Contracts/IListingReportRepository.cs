using rent_for_students.Domain.Reports;

namespace rent_for_students.Domain.Contracts
{
    public interface IListingReportRepository
    {
        Task<IReadOnlyList<ListingDemandReportRow>> GetDemandReportAsync(CancellationToken ct = default);
    }
}
