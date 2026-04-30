using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class UpdateListingCommand : ICommand<bool>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly Guid _listingId;
        private readonly HousingListing _listing;

        public UpdateListingCommand(IListingUseCaseMediator receiver, Guid listingId, HousingListing listing)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
            _listing = listing ?? throw new ArgumentNullException(nameof(listing));
        }

        public Task<Result<bool>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.UpdateAsync(_listingId, _listing, ct);
    }
}
