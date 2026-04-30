using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class GetListingApplicationsCommand : ICommand<IReadOnlyList<RentalApplication>>
    {
        private readonly IApplicationUseCaseMediator _receiver;
        private readonly Guid _listingId;

        public GetListingApplicationsCommand(IApplicationUseCaseMediator receiver, Guid listingId)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
        }

        public Task<Result<IReadOnlyList<RentalApplication>>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.ListByListingIdAsync(_listingId, ct);
    }
}
