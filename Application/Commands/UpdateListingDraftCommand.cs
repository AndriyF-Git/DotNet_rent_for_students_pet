using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class UpdateListingDraftCommand : ICommand<bool>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly Guid _listingId;
        private readonly HousingListing _draft;

        public UpdateListingDraftCommand(IListingUseCaseMediator receiver, Guid listingId, HousingListing draft)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
            _draft = draft ?? throw new ArgumentNullException(nameof(draft));
        }

        public Task<Result<bool>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.UpdateDraftAsync(_listingId, _draft, ct);
    }
}
