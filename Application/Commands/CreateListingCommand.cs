using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class CreateListingCommand : ICommand<Guid>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly HousingListing _listing;

        public CreateListingCommand(IListingUseCaseMediator receiver, HousingListing listing)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listing = listing ?? throw new ArgumentNullException(nameof(listing));
        }

        public Task<Result<Guid>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.CreateAsync(_listing, ct);
    }
}
