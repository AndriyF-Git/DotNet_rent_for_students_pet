using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;

namespace rent_for_students.Application.Commands
{
    // PROMPT v1.3: Prototype usage command - apply from saved profile.
    public sealed class CreateRentalApplicationFromProfileCommand : ICommand<Guid>
    {
        private readonly IApplicationUseCaseMediator _receiver;
        private readonly Guid _listingId;
        private readonly Guid _profileId;

        public CreateRentalApplicationFromProfileCommand(IApplicationUseCaseMediator receiver, Guid listingId, Guid profileId)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
            _profileId = profileId;
        }

        public Task<Result<Guid>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.ApplyFromProfileAsync(_listingId, _profileId, ct);
    }
}
