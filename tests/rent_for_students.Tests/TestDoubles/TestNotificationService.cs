using rent_for_students.Application.Notifications;

namespace rent_for_students.Tests.TestDoubles
{
    internal sealed class TestNotificationService : INotificationService
    {
        public List<string> Messages { get; } = new();

        public Task NotifyAsync(string message, CancellationToken ct = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
