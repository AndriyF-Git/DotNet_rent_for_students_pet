namespace rent_for_students.Application.Notifications
{
    public interface INotificationService
    {
        Task NotifyAsync(string message, CancellationToken ct = default);
    }
}
