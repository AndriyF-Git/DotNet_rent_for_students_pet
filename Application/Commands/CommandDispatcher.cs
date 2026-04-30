using rent_for_students.Application.Common;

namespace rent_for_students.Application.Commands
{
    public class CommandDispatcher
    {
        public Task<Result<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(command);
            return command.ExecuteAsync(ct);
        }
    }
}
