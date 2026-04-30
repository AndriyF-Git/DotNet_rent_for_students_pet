using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Contracts;

namespace rent_for_students.Application.Commands
{
    // PROMPT v1.3: Prototype source retrieval command.
    public sealed class GetRentalApplicationProfilesCommand : ICommand<IReadOnlyList<IRentalApplicationPrototype>>
    {
        private readonly IApplicationUseCaseMediator _receiver;

        public GetRentalApplicationProfilesCommand(IApplicationUseCaseMediator receiver)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        }

        public Task<Result<IReadOnlyList<IRentalApplicationPrototype>>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.ListProfilesAsync(ct);
    }
}
