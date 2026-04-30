using rent_for_students.Domain.Reports;

namespace rent_for_students.ViewModels
{
    public class ListingDemandReportViewModel
    {
        public IReadOnlyList<ListingDemandReportRow> Rows { get; init; } = Array.Empty<ListingDemandReportRow>();
    }
}
