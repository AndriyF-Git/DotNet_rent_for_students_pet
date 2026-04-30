using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Reports;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    // PROMPT v1.7: Report repository — calls sp_GenerateListingDemandReport (cursor-based SP)
    public class SpListingReportRepository : IListingReportRepository
    {
        private readonly AppDbContext _db;

        public SpListingReportRepository(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IReadOnlyList<ListingDemandReportRow>> GetDemandReportAsync(CancellationToken ct = default)
        {
            List<ListingDemandReportRow> rows = await _db.Database
                .SqlQueryRaw<ListingDemandReportRow>("EXEC sp_GenerateListingDemandReport")
                .ToListAsync(ct);

            return rows;
        }
    }
}
