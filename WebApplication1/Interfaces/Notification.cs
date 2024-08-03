using WebApplication1.Models;

namespace WebApplication1.Interfaces
{
    public class Notification : INotificationService
    {
        public void NotifyUsernameChanged(User user)
        {
            Console.WriteLine($"Username has been changed to : {user.firstname}");
        }
    }

    public class NotificationAdvanced : INotificationService
    {
        public void NotifyUsernameChanged(User user)
        {
            Console.WriteLine($"ADVANCED: Username has been changed to : {user.firstname}");
        }
    }
}
