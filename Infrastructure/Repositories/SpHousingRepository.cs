using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Requests;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    // PROMPT v1.6: SP-based repository — delegates CRUD to stored procedures
    public class SpHousingRepository : IHousingRepository
    {
        private readonly AppDbContext _db;

        public SpHousingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", id)
            };

            List<HousingListing> results = await _db.HousingListings
                .FromSqlRaw("EXEC sp_HousingListings_GetById @Id", parameters)
                .AsNoTracking()
                .ToListAsync(ct);

            return results.FirstOrDefault();
        }

        public async Task<IReadOnlyList<HousingListing>> SearchAsync(
            ListingSearchCriteria criteria,
            CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@City", (object?)criteria.City ?? DBNull.Value),
                new SqlParameter("@MaxPrice", (object?)criteria.MaxPricePerMonth ?? DBNull.Value),
                new SqlParameter("@RoomType", criteria.RoomType.HasValue ? (int)criteria.RoomType.Value : DBNull.Value),
                new SqlParameter("@OnlyActive", criteria.OnlyActive ?? true)
            };

            List<HousingListing> results = await _db.HousingListings
                .FromSqlRaw("EXEC sp_HousingListings_GetAll @City, @MaxPrice, @RoomType, @OnlyActive", parameters)
                .AsNoTracking()
                .ToListAsync(ct);

            // Client-side paging (SP returns filtered set, paging applied here)
            int skip = (criteria.Page - 1) * criteria.PageSize;
            return results.Skip(skip).Take(criteria.PageSize).ToList();
        }

        public async Task AddAsync(HousingListing listing, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", listing.Id),
                new SqlParameter("@Title", listing.Title),
                new SqlParameter("@Description", (object?)listing.Description ?? DBNull.Value),
                new SqlParameter("@City", listing.City),
                new SqlParameter("@District", (object?)listing.District ?? DBNull.Value),
                new SqlParameter("@PricePerMonth", listing.PricePerMonth),
                new SqlParameter("@RoomType", (int)listing.RoomType),
                new SqlParameter("@AreaSqm", listing.AreaSqm),
                new SqlParameter("@IsActive", listing.IsActive),
                new SqlParameter("@CreatedAtUtc", listing.CreatedAtUtc)
            };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC sp_HousingListings_Create @Id, @Title, @Description, @City, @District, @PricePerMonth, @RoomType, @AreaSqm, @IsActive, @CreatedAtUtc",
                parameters,
                ct);
        }

        public async Task UpdateAsync(HousingListing listing, CancellationToken ct = default)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", listing.Id),
                new SqlParameter("@Title", listing.Title),
                new SqlParameter("@Description", (object?)listing.Description ?? DBNull.Value),
                new SqlParameter("@City", listing.City),
                new SqlParameter("@District", (object?)listing.District ?? DBNull.Value),
                new SqlParameter("@PricePerMonth", listing.PricePerMonth),
                new SqlParameter("@RoomType", (int)listing.RoomType),
                new SqlParameter("@AreaSqm", listing.AreaSqm),
                new SqlParameter("@IsActive", listing.IsActive)
            };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC sp_HousingListings_Update @Id, @Title, @Description, @City, @District, @PricePerMonth, @RoomType, @AreaSqm, @IsActive",
                parameters,
                ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            // SP commits immediately — no-op for compatibility
            return Task.CompletedTask;
        }
    }
}
