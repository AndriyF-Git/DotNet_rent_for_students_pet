using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;

namespace rent_for_students.Application.Commands
{
    public sealed class PublishListingCommand : ICommand<bool>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly Guid _listingId;

        public PublishListingCommand(IListingUseCaseMediator receiver, Guid listingId)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
        }

        public Task<Result<bool>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.PublishAsync(_listingId, ct);
    }
}
