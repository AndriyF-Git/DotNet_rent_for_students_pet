using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    // PROMPT v1.6: SP-based repository — delegates CRUD to stored procedures
    public class SpRentalApplicationRepository : IRentalApplicationRepository
    {
        private readonly AppDbContext _db;

        public SpRentalApplicationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RentalApplication application, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", application.Id),
                new SqlParameter("@ListingId", application.ListingId),
                new SqlParameter("@ApplicantName", application.ApplicantName),
                new SqlParameter("@Phone", application.Phone),
                new SqlParameter("@Email", application.Email),
                new SqlParameter("@Message", (object?)application.Message ?? DBNull.Value),
                new SqlParameter("@Status", (int)application.Status),
                new SqlParameter("@CreatedAtUtc", application.CreatedAtUtc)
            };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC sp_RentalApplications_Create @Id, @ListingId, @ApplicantName, @Phone, @Email, @Message, @Status, @CreatedAtUtc",
                parameters,
                ct);
        }

        public async Task<RentalApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            List<RentalApplication> results = await _db.RentalApplications
                .FromSqlRaw("EXEC sp_RentalApplications_GetById @Id", parameters)
                .AsNoTracking()
                .ToListAsync(ct);

            return results.FirstOrDefault();
        }

        public async Task<IReadOnlyList<RentalApplication>> ListByListingIdAsync(
            Guid listingId,
            CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ListingId", listingId)
            };

            List<RentalApplication> results = await _db.RentalApplications
                .FromSqlRaw("EXEC sp_RentalApplications_GetByListingId @ListingId", parameters)
                .AsNoTracking()
                .ToListAsync(ct);

            return results;
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            // SP commits immediately — no-op for compatibility
            return Task.CompletedTask;
        }
    }
}
