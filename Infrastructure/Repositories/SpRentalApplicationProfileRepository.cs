using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    // PROMPT v1.6: SP-based repository — delegates CRUD to stored procedures
    public class SpRentalApplicationProfileRepository : IRentalApplicationProfileRepository
    {
        private readonly AppDbContext _db;

        public SpRentalApplicationProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", prototype.Id),
                new SqlParameter("@ProfileName", prototype.ProfileName),
                new SqlParameter("@ApplicantName", prototype.ApplicantName),
                new SqlParameter("@Phone", prototype.Phone),
                new SqlParameter("@Email", prototype.Email),
                new SqlParameter("@Message", (object?)prototype.Message ?? DBNull.Value),
                new SqlParameter("@CreatedAtUtc", prototype.CreatedAtUtc)
            };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC sp_RentalApplicationProfiles_Create @Id, @ProfileName, @ApplicantName, @Phone, @Email, @Message, @CreatedAtUtc",
                parameters,
                ct);
        }

        public async Task<IRentalApplicationPrototype?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            List<RentalApplicationProfile> results = await _db.RentalApplicationProfiles
                .FromSqlRaw("EXEC sp_RentalApplicationProfiles_GetById @Id", parameters)
                .AsNoTracking()
                .ToListAsync(ct);

            return results.FirstOrDefault();
        }

        public async Task<IReadOnlyList<IRentalApplicationPrototype>> ListAsync(CancellationToken ct = default)
        {
            List<RentalApplicationProfile> profiles = await _db.RentalApplicationProfiles
                .FromSqlRaw("EXEC sp_RentalApplicationProfiles_GetAll")
                .AsNoTracking()
                .ToListAsync(ct);

            return profiles.Cast<IRentalApplicationPrototype>().ToList();
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            // SP commits immediately — no-op for compatibility
            return Task.CompletedTask;
        }
    }
}
