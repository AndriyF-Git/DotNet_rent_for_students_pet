using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;
using rent_for_students.Domain.Requests;

namespace rent_for_students.Application.Commands
{
    public sealed class SearchListingsCommand : ICommand<IReadOnlyList<HousingListing>>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly ListingSearchCriteria _criteria;

        public SearchListingsCommand(IListingUseCaseMediator receiver, ListingSearchCriteria criteria)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        }

        public Task<Result<IReadOnlyList<HousingListing>>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.SearchListingsAsync(_criteria, ct);
    }
}
