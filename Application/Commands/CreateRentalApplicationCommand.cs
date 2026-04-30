using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Application.Commands
{
    public sealed class CreateRentalApplicationCommand : ICommand<Guid>
    {
        private readonly IApplicationUseCaseMediator _receiver;
        private readonly Guid _listingId;
        private readonly RentalApplication _applicant;

        public CreateRentalApplicationCommand(IApplicationUseCaseMediator receiver, Guid listingId, RentalApplication applicant)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _listingId = listingId;
            _applicant = applicant ?? throw new ArgumentNullException(nameof(applicant));
        }

        public Task<Result<Guid>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.ApplyAsync(_listingId, _applicant, ct);
    }
}
