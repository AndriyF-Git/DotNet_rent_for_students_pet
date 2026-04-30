using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Reports;

namespace rent_for_students.Tests.TestDoubles
{
    public class StubListingReportRepository : IListingReportRepository
    {
        public Task<IReadOnlyList<ListingDemandReportRow>> GetDemandReportAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ListingDemandReportRow>>(Array.Empty<ListingDemandReportRow>());
    }
}
