using rent_for_students.Application.Common;

namespace rent_for_students.Application.Commands
{
    public interface ICommand<TResult>
    {
        Task<Result<TResult>> ExecuteAsync(CancellationToken ct = default);
    }
}
