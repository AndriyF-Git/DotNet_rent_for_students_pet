using rent_for_students.Domain.Contracts;
using rent_for_students.Domain.Entities;

namespace rent_for_students.Tests.TestDoubles
{
    internal sealed class InMemoryRentalApplicationProfileRepository : IRentalApplicationProfileRepository
    {
        private readonly List<IRentalApplicationPrototype> _items = new();

        public Task AddAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default)
        {
            _items.Add(prototype);
            return Task.CompletedTask;
        }

        public Task<IRentalApplicationPrototype?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task<IReadOnlyList<IRentalApplicationPrototype>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<IRentalApplicationPrototype>>(
                _items.OrderBy(x => x.ProfileName).ThenByDescending(x => x.UpdatedAtUtc).ToList());

        public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
