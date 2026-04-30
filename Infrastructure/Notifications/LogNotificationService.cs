using rent_for_students.Application.Notifications;

namespace rent_for_students.Infrastructure.Notifications
{
    public class LogNotificationService : INotificationService
    {
        private readonly ILogger<LogNotificationService> _logger;

        public LogNotificationService(ILogger<LogNotificationService> logger)
        {
            _logger = logger;
        }

        public Task NotifyAsync(string message, CancellationToken ct = default)
        {
            _logger.LogInformation("Notification: {Message}", message);
            return Task.CompletedTask;
        }
    }
}
