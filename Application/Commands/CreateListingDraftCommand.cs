using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class CreateListingDraftCommand : ICommand<Guid>
    {
        private readonly IListingUseCaseMediator _receiver;
        private readonly HousingListing _draft;

        public CreateListingDraftCommand(IListingUseCaseMediator receiver, HousingListing draft)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _draft = draft ?? throw new ArgumentNullException(nameof(draft));
        }

        public Task<Result<Guid>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.CreateDraftAsync(_draft, ct);
    }
}
