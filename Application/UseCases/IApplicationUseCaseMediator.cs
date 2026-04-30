using rent_for_students.Application.Common;
using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.UseCases
{
    public interface IApplicationUseCaseMediator
    {
        Task<Result<Guid>> ApplyAsync(Guid listingId, RentalApplication applicant, CancellationToken ct = default);
        Task<Result<Guid>> ApplyFromProfileAsync(Guid listingId, Guid profileId, CancellationToken ct = default);
        Task<Result<Guid>> CreateProfileAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default);
        Task<Result<IReadOnlyList<IRentalApplicationPrototype>>> ListProfilesAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<RentalApplication>>> ListByListingIdAsync(Guid listingId, CancellationToken ct = default);
    }
}
