using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Reports;

namespace rent_for_students.Application.Commands
{
    public sealed class GetListingDemandReportCommand : ICommand<IReadOnlyList<ListingDemandReportRow>>
    {
        private readonly IListingUseCaseMediator _receiver;

        public GetListingDemandReportCommand(IListingUseCaseMediator receiver)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        public Task<Result<IReadOnlyList<ListingDemandReportRow>>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.GetDemandReportAsync(ct);
    }
}
