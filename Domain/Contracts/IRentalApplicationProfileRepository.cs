namespace rent_for_students.Domain.Contracts
{
    public interface IRentalApplicationProfileRepository
    {
        Task AddAsync(IRentalApplicationPrototype prototype, CancellationToken ct = default);
        Task<IRentalApplicationPrototype?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<IRentalApplicationPrototype>> ListAsync(CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
