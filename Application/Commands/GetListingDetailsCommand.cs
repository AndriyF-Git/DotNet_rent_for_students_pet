using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class GetListingDetailsCommand : ICommand<HousingListing>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly Guid _listingId;

        public GetListingDetailsCommand(IListingUseCaseMediator receiver, Guid listingId)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
        }

        public Task<Result<HousingListing>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.GetDetailsAsync(_listingId, ct);
    }
}
