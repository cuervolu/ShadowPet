using Avalonia.Controls;
using Avalonia.Controls.Notifications;
namespace ShadowPet.Desktop.Services
{
    public class NotificationService
    {
        private WindowNotificationManager? _notificationManager;

        public void SetHostWindow(TopLevel? host)
        {
            if (host != null)
            {
                _notificationManager = new WindowNotificationManager(host)
                {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3,
                    Margin = new(0, 0, 15, 15)
                };
            }
        }

        public void Show(string title, string message, NotificationType type = NotificationType.Information)
        {
            if (_notificationManager is null) return;

            _notificationManager.Show(new Notification(title, message, type));
        }
    }
}
