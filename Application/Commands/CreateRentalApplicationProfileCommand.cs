using rent_for_students.Application.Common;
using rent_for_students.Application.UseCases;
using rent_for_students.Domain.Contracts;

namespace rent_for_students.Application.Commands
{
    // PROMPT v1.3: Prototype source management command.
    public sealed class CreateRentalApplicationProfileCommand : ICommand<Guid>
    {
        private readonly IApplicationUseCaseMediator _receiver;
        private readonly IRentalApplicationPrototype _prototype;

        public CreateRentalApplicationProfileCommand(IApplicationUseCaseMediator receiver, IRentalApplicationPrototype prototype)
        {
            _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            _prototype = prototype ?? throw new ArgumentNullException(nameof(prototype));
        }

        public Task<Result<Guid>> ExecuteAsync(CancellationToken ct = default)
            => _receiver.CreateProfileAsync(_prototype, ct);
    }
}
