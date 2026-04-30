using Microsoft.EntityFrameworkCore;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;
using rent_for_students.Infrastructure.Data;

namespace rent_for_students.Infrastructure.Repositories
{
    public class EfRentalApplicationProfileRepository : IRentalApplicationProfileRepository
    {
        private readonly AppDbContext _db;

        public EfRentalApplicationProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default)
        {
            RentalApplicationProfile entity = ToEntity(prototype);
            return _db.RentalApplicationProfiles.AddAsync(entity, ct).AsTask();
        }

        public async Task<IRentalApplicationPrototype?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.RentalApplicationProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<IReadOnlyList<IRentalApplicationPrototype>> ListAsync(CancellationToken ct = default)
        {
            List<RentalApplicationProfile> profiles = await _db.RentalApplicationProfiles
                .AsNoTracking()
                .OrderBy(x => x.ProfileName)
                .ThenByDescending(x => x.UpdatedAtUtc)
                .ToListAsync(ct);
            return profiles.Cast<IRentalApplicationPrototype>().ToList();
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        private static RentalApplicationProfile ToEntity(IRentalApplicationPrototype prototype)
        {
            if (prototype is RentalApplicationProfile profile)
            {
                return profile;
            }

            return new RentalApplicationProfile
            {
                Id = prototype.Id,
                ProfileName = prototype.ProfileName,
                ApplicantName = prototype.ApplicantName,
                Phone = prototype.Phone,
                Email = prototype.Email,
                Message = prototype.Message,
                CreatedAtUtc = prototype.CreatedAtUtc,
                UpdatedAtUtc = prototype.UpdatedAtUtc
            };
        }
    }
}
